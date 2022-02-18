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
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts
{
    public class ScriptManager : IDisposable
    {
        public IReadOnlyList<IScriptLanguage> Languages => languages;
        private readonly List<IScriptLanguage> languages = new List<IScriptLanguage>();
        protected bool IsDisposed { get; private set; }

        public ScriptManager(Storage storage)
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            languages.Add(new BuiltinScriptLanguage());

            foreach (var type in loadedTypeFromAssembly)
            {
                if (type.IsAssignableTo(typeof(FileBasedScriptLanguage)))
                    languages.Add(Activator.CreateInstance(type, new object[] { storage }) as IScriptLanguage);
                else
                    languages.Add(Activator.CreateInstance(type) as IScriptLanguage);
            }
        }

        public void AddLanguage(IScriptLanguage language) => languages.Add(language);
        public void RemoveLanguage(IScriptLanguage language) => languages.Remove(language);

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

        public static readonly IReadOnlyList<Assembly> Loaded;
        private static readonly List<Type> loadedTypeFromAssembly = new List<Type>();

        static ScriptManager()
        {
            string[] files = Directory.GetFiles(RuntimeInfo.StartupDirectory, @"sbtw.Editor.Scripts.*.dll");
            var loadedAssemblies = new List<Assembly>();

            foreach (string file in files)
            {
                var assembly = Assembly.LoadFrom(file);
                var type = assembly.GetTypes().First(t => t.IsPublic && t.IsSubclassOf(typeof(ScriptLanguage)));

                if (type != null)
                {
                    loadedAssemblies.Add(assembly);
                    loadedTypeFromAssembly.Add(type);
                }
            }

            Loaded = loadedAssemblies;
        }
    }
}
