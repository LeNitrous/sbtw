// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using osu.Framework.Audio.Track;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using sbtw.Editor.Scripts.Graphics;

namespace sbtw.Editor.Scripts
{
    public abstract class Script : IDisposable
    {
        private static readonly Dictionary<MethodInfo, Type> importable_methods = new Dictionary<MethodInfo, Type>();
        private static readonly List<PropertyInfo> importable_properties = new List<PropertyInfo>();
        private static readonly Type[] importable_types = new[]
        {
            typeof(Text),
            typeof(Layer),
            typeof(Rectangle),
            typeof(FontConfiguration),
            typeof(osuTK.Vector2),
            typeof(osu.Framework.Graphics.Anchor),
            typeof(osu.Framework.Graphics.Easing),
            typeof(osu.Framework.Graphics.Colour4),
            typeof(osu.Game.Storyboards.AnimationLoopType),
        };

        static Script()
        {
            var types = new Dictionary<string, Type>();

            foreach (var type in typeof(Delegate).Assembly.GetTypes().Where(t => t.Name.StartsWith("Action") || t.Name.StartsWith("Func")))
                types.Add(type.Name, type);

            var members = typeof(Script).GetMembers().Where(m => m.GetCustomAttributes(typeof(VisibleAttribute), false).Length > 0);

            foreach (var method in members.OfType<MethodInfo>())
            {
                bool isFunc = method.ReturnType != typeof(void);
                int parameterCount = method.GetParameters().Length;
                int genericCount = isFunc ? parameterCount + 1 : parameterCount;

                string key = isFunc ? "Func" : "Action";
                key += parameterCount > 0 ? $"`{genericCount}" : string.Empty;

                Type producedType = types[key];

                if (parameterCount > 0)
                {
                    var genericTypes = method.GetParameters().Select(p => p.ParameterType);

                    if (isFunc)
                        genericTypes = genericTypes.Append(method.ReturnType);

                    producedType = producedType.MakeGenericType(genericTypes.ToArray());
                }

                importable_methods.Add(method, producedType);
            }

            foreach (var property in members.OfType<PropertyInfo>())
                importable_properties.Add(property);
        }

        public readonly string Name;
        public readonly string Path;

        private readonly List<ScriptVariableInfo> internalVariables = new List<ScriptVariableInfo>();
        private readonly List<ScriptVariableInfo> variables = new List<ScriptVariableInfo>();
        private readonly List<ScriptElementGroup> groups = new List<ScriptElementGroup>();
        protected readonly Logger Logger = Logger.GetLogger("script");

        public Storage Storage { get; private set; }

        [Visible]
        public IBeatmap Beatmap { get; private set; }

        [Visible]
        public Waveform Waveform { get; private set; }

        protected bool IsDisposed { get; private set; }

        protected Script(string name, string path)
        {
            Name = name.Pascalize();
            Path = path;
        }

        [Visible]
        public string GetAsset(string path, Asset asset) => asset.Generate(this, path);

        [Visible]
        public ScriptElementGroup GetGroup(string name)
        {
            var group = groups.FirstOrDefault(g => g.Name == name);

            if (group != null)
                return group;

            group = new ScriptElementGroup(this, name);
            groups.Add(group);

            return group;
        }

        [Visible]
        public void SetVideo(string path, int offset)
        {
            if (groups.Any(g => g.Name == "Video"))
                throw new InvalidOperationException("Cannot create another video in the same script.");

            GetGroup("Video").CreateVideo(path, offset);
        }

        [Visible]
        public object SetValue(string name, object value)
        {
            var variable = internalVariables.FirstOrDefault(v => v.Name == name);

            if (variable != null)
                variables.Add(variable);

            variable ??= variables.FirstOrDefault(v => v.Name == name);

            if (variable == null)
                variables.Add(variable = new ScriptVariableInfo(name, value));

            return variable.Value;
        }

        public T SetValue<T>(string name, T value)
            => ensure<T>(SetValue(name, (object)value));

        [Visible]
        public object GetValue(string name)
        {
            var variable = internalVariables.FirstOrDefault(v => v.Name == name) ?? variables.FirstOrDefault(v => v.Name == name);

            if (variable == null)
                return null;

            return variable.Value;
        }

        public T GetValue<T>(string name)
            => ensure<T>(GetValue(name));

        private static T ensure<T>(object obj)
        {
            if (obj is not T)
                throw new InvalidCastException($"Cannot convert {obj.GetType()} to {typeof(T)}");

            return (T)obj;
        }

        public void Log(string message, LogLevel level)
            => Logger.Add($"[{System.IO.Path.GetFileName(Path)}]: {message}", level);

        public void Error(string message)
            => Log(message, LogLevel.Error);

        [Visible]
        public void Log(string message)
            => Log(message, LogLevel.Debug);

        [Visible]
        public byte[] OpenFile(string path)
        {
            if (Storage?.Exists(path) ?? false)
                return null;

            byte[] data = null;

            using (var stream = Storage.GetStream(path, mode: FileMode.Open))
            {
                using var reader = new MemoryStream();
                stream.CopyTo(reader);
                data = reader.ToArray();
            }

            return data;
        }

        public void SetValueInternal(string name, object value)
            => internalVariables.Add(new ScriptVariableInfo(name, value));

        public ScriptGenerationResult Generate(Storage storage = null, IBeatmap beatmap = null, Waveform waveform = null)
        {
            if (IsDisposed)
                throw new ObjectDisposedException($"{this} is already disposed and cannot generate.");

            Beatmap = beatmap;
            Waveform = waveform;
            Storage = storage;

            importTypesAndMembers();
            Compile();

            bool faulted = false;

            try
            {
                Perform();
            }
            catch (Exception ex)
            {
                faulted = true;
                Logger.Add(FormatErrorMessage(ex), LogLevel.Error);
            }

            return new ScriptGenerationResult { Name = Name, Groups = groups, Variables = variables, Faulted = faulted };
        }

        public Task<ScriptGenerationResult> GenerateAsync(Storage storage = null, IBeatmap beatmap = null, Waveform waveform = null, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            return Task.Run(() => Generate(storage, beatmap, waveform), token);
        }

        protected virtual string FormatErrorMessage(Exception exception) => exception.ToString();

        protected virtual void Compile()
        {
        }

        protected abstract void Perform();
        protected abstract void RegisterMethod(string name, Delegate method);
        protected abstract void RegisterField(string name, object value);
        protected abstract void RegisterType(Type type);

        private void importTypesAndMembers()
        {
            foreach ((MethodInfo method, Type methodType) in importable_methods)
                RegisterMethod(method.Name, method.CreateDelegate(methodType, this));

            foreach (PropertyInfo property in importable_properties)
                RegisterField(property.Name, property.GetValue(this));

            foreach (Type type in importable_types)
                RegisterType(type);
        }

        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString() => @$"{GetType().Name} ({Path})";
    }
}
