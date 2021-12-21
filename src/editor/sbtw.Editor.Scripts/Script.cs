// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    public abstract class Script
    {
        public virtual string Name => GetType().Name;
        protected internal readonly List<ScriptVariableInfo> Variables = new List<ScriptVariableInfo>();
        protected internal readonly List<ScriptElementGroup> Groups = new List<ScriptElementGroup>();

        protected abstract void Perform();

        public ScriptElementGroup GetGroup(string name)
        {
            var group = Groups.FirstOrDefault(g => g.Name == name);

            if (group != null)
                return group;

            group = new ScriptElementGroup(this, name);
            Groups.Add(group);

            return group;
        }

        public void SetVideo(string path, int offset)
        {
            if (Groups.Any(g => g.Name == "Video"))
                throw new InvalidOperationException("Cannot create another video in the same script.");

            GetGroup("Video").CreateVideo(path, offset);
        }

        public object SetValue(string name, object value)
        {
            var variable = Variables.FirstOrDefault(v => v.Name == name);

            if (variable == default)
                Variables.Add(variable = new ScriptVariableInfo(name, value));

            return variable.Value;
        }

        public T SetValue<T>(string name, T value)
            => ensure<T>(SetValue(name, (object)value));

        public object GetValue(string name)
        {
            var variable = Variables.FirstOrDefault(v => v.Name == name);

            if (variable == default)
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

        internal void SetInternal(string name, object value)
        {
            var variable = Variables.FirstOrDefault(v => v.Name == name);

            if (variable != default)
                Variables.Remove(variable);

            Variables.Add(new ScriptVariableInfo(name, value));
        }

        internal ScriptGenerationResult Generate()
        {
            Compile();
            Perform();
            return new ScriptGenerationResult { Name = Name, Groups = Groups, Variables = Variables };
        }

        internal Task<ScriptGenerationResult> GenerateAsync(CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            return Task.Run(Generate, token);
        }

        protected internal virtual void Compile()
        {
        }
    }
}
