// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    public class ScriptManager : IDisposable
    {
        public IReadOnlyList<IScriptLanguage> Languages => languages;
        private readonly IProject project;
        private readonly List<IScriptLanguage> languages = new List<IScriptLanguage>();
        protected bool IsDisposed { get; private set; }

        public ScriptManager(IProject project, IEnumerable<Type> types, bool loadAssemblies = false)
        {
            this.project = project ?? throw new ArgumentNullException(nameof(project));

            if (this.project is not ICanProvideFiles)
                throw new ArgumentException($"{nameof(IProject)} does not implement {nameof(ICanProvideFiles)}.");

            foreach (var type in types ?? throw new ArgumentNullException(nameof(types)))
            {
                if (!type.IsAssignableTo(typeof(IScriptLanguage)))
                    throw new ArgumentException($"{type.Name} is not a {nameof(IScriptLanguage)}.", nameof(types));

                if (type.GetConstructor(new[] { typeof(IProject) }) == null)
                    throw new ArgumentException($"{type.Name} does not have the required constructor.");

                languages.Add(Activator.CreateInstance(type, new object[] { project }) as IScriptLanguage);
            }

            if (!loadAssemblies)
                return;

            string[] files = Directory.GetFiles(RuntimeInfo.StartupDirectory, @"sbtw.Editor.Scripts.*.dll");

            foreach (string file in files)
            {
                var assembly = Assembly.LoadFrom(file);
                var type = assembly.GetTypes().First(t => t.IsPublic && t.IsSubclassOf(typeof(ScriptLanguage)));

                if (type != null)
                    languages.Add(Activator.CreateInstance(type, new object[] { project }) as IScriptLanguage);
            }
        }

        public IEnumerable<ScriptExecutionResult> Execute()
            => Execute<object>(null);

        public Task<IEnumerable<ScriptExecutionResult>> ExecuteAsync(CancellationToken token = default)
            => ExecuteAsync<object>(null, token);

        public IEnumerable<ScriptExecutionResult> Execute<T>(T globals)
            where T : class
        {
            return ExecuteAsync(globals).Result;
        }

        public async Task<IEnumerable<ScriptExecutionResult>> ExecuteAsync<T>(T globals, CancellationToken token = default)
            where T : class
        {
            return (await Task.WhenAll(languages.Select(s => s.ExecuteAsync(globals, token)))).SelectMany(s => s);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            foreach (var lang in languages)
                lang.Dispose();

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
