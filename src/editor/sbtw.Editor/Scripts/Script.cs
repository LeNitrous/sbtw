// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using osu.Framework.Logging;

namespace sbtw.Editor.Scripts
{
    public abstract class Script
    {
        private static readonly Dictionary<MethodInfo, Type> visible_member_types = new Dictionary<MethodInfo, Type>();

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

                visible_member_types.Add(method, producedType);
            }
        }

        public readonly string Name;
        public readonly string Path;

        private readonly List<ScriptVariableInfo> internalVariables = new List<ScriptVariableInfo>();
        private readonly List<ScriptVariableInfo> variables = new List<ScriptVariableInfo>();
        private readonly List<ScriptElementGroup> groups = new List<ScriptElementGroup>();
        protected readonly Logger Logger = Logger.GetLogger("script");

        protected abstract void Perform();

        protected Script(string name, string path)
        {
            Name = name.Pascalize();
            Path = path;
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

        [Visible]
        public void Log(string message)
            => Logger.Debug($"[{System.IO.Path.GetFileName(Path)}]: {message}");

        public void SetValueInternal(string name, object value)
            => internalVariables.Add(new ScriptVariableInfo(name, value));

        public ScriptGenerationResult Generate()
        {
            shareVisibleMembers();
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

        public Task<ScriptGenerationResult> GenerateAsync(CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            return Task.Run(Generate, token);
        }

        protected virtual string FormatErrorMessage(Exception exception) => exception.ToString();

        protected virtual void Compile()
        {
        }

        protected abstract void RegisterMethod(string name, Delegate method);

        private void shareVisibleMembers()
        {
            foreach ((MethodInfo method, Type methodType) in visible_member_types)
                RegisterMethod(method.Name, method.CreateDelegate(methodType, this));
        }
    }
}
