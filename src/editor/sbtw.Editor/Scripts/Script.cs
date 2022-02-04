// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using osu.Framework.Audio.Track;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using sbtw.Editor.Assets;

namespace sbtw.Editor.Scripts
{
    public abstract class Script : IDisposable
    {
        private static readonly List<MethodInfo> importable_methods = new List<MethodInfo>();
        private static readonly List<PropertyInfo> importable_properties = new List<PropertyInfo>();
        private static readonly Type[] importable_types = new[]
        {
            typeof(Text),
            typeof(Rectangle),
            typeof(Layer),
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
                importable_methods.Add(method);

            foreach (var property in members.OfType<PropertyInfo>())
                importable_properties.Add(property);
        }

        public readonly string Name;
        public readonly string Path;

        private readonly List<ScriptElementGroup> groups = new List<ScriptElementGroup>();
        private readonly List<Asset> assets = new List<Asset>();
        private readonly Logger logger = Logger.GetLogger("script");

        public Storage Storage { get; private set; }

        [Visible]
        public IBeatmap Beatmap { get; private set; }

        public Waveform Waveform { get; private set; }

        protected bool IsDisposed { get; private set; }

        protected Script(string name, string path)
        {
            Name = name.Pascalize();
            Path = path;
        }

        [Visible]
        public string GetAsset(string path, Asset asset)
        {
            asset.Register(this, path);
            assets.Add(asset);
            return path;
        }

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

        public void Log(object message, LogLevel level)
            => logger.Add($"[{System.IO.Path.GetFileName(Path)}]: {message}", level);

        [Visible]
        public void Log(object message)
            => Log(message, LogLevel.Debug);

        [Visible]
        public void Error(object message)
            => Log(message, LogLevel.Error);

        [Visible]
        public byte[] OpenFile(string path)
        {
            if (!Storage?.Exists(path) ?? false)
                throw new FileNotFoundException($@"File ""{path}"" does not exist.");

            byte[] data = null;

            using (var stream = Storage.GetStream(path, mode: FileMode.Open))
            {
                using var reader = new MemoryStream();
                stream.CopyTo(reader);
                data = reader.ToArray();
            }

            return data;
        }

        [Visible]
        public string OpenFileAsText(string path)
            => Encoding.Default.GetString(OpenFile(path));

        public ScriptGenerationResult Generate(Storage storage = null, IBeatmap beatmap = null, Waveform waveform = null)
            => GenerateAsync(storage, beatmap, waveform).Result;

        public async Task<ScriptGenerationResult> GenerateAsync(Storage storage = null, IBeatmap beatmap = null, Waveform waveform = null, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            if (IsDisposed)
                throw new ObjectDisposedException($"{this} is already disposed and cannot generate.");

            Beatmap = beatmap;
            Storage = storage;
            Waveform = waveform;

            foreach (MethodInfo method in importable_methods)
            {
                var parameters = method.GetParameters().Select(p => p.ParameterType);
                var type = method.ReturnType == typeof(void)
                    ? Expression.GetActionType(parameters.ToArray())
                    : Expression.GetFuncType(parameters.Append(method.ReturnType).ToArray());

                RegisterMethod(method.Name, method.CreateDelegate(type, this));
            }

            foreach (PropertyInfo property in importable_properties)
                RegisterField(property.Name, property.GetValue(this));

            foreach (Type type in importable_types)
                RegisterType(type);

            bool faulted = true;

            try
            {
                await PerformAsync();
                faulted = false;
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.Flatten().InnerExceptions)
                    Error(FormatException(ex));
            }
            catch (Exception ex)
            {
                Error(FormatException(ex));
            }

            return new ScriptGenerationResult { Name = Name, Path = Path, Assets = assets, Groups = groups, Faulted = faulted };
        }

        protected virtual string FormatException(Exception ex) => ex.Message;
        protected abstract Task PerformAsync();
        protected abstract void RegisterMethod(string name, Delegate method);
        protected abstract void RegisterField(string name, object value);
        protected abstract void RegisterType(Type type);

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
