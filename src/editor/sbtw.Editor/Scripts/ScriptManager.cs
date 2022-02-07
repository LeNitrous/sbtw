// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    public class ScriptManager : IDisposable
    {
        private readonly IProject project;
        private readonly List<IScriptLanguage> languages = new List<IScriptLanguage>();
        protected bool IsDisposed { get; private set; }

        public ScriptManager(IProject project, IEnumerable<Type> types)
        {
            this.project = project ?? throw new ArgumentNullException(nameof(project));

            if (this.project is not ICanProvideFiles)
                throw new ArgumentException($"{nameof(IProject)} does not implement {nameof(ICanProvideFiles)}.");

            foreach (var type in types ?? throw new ArgumentNullException(nameof(types)))
            {
                if (!type.IsAssignableFrom(typeof(IScriptLanguage)))
                    throw new ArgumentException($"{type.Name} is not a {nameof(IScriptLanguage)}.", nameof(types));

                if (type.GetConstructor(new[] { typeof(IProject) }) == null)
                    throw new ArgumentException($"{type.Name} does not have a matching constructor.");

                languages.Add(Activator.CreateInstance(type, new object[] { project }) as IScriptLanguage);
            }
        }

        public IEnumerable<IScript> GetScripts(ScriptResources resources) => GetScriptsAsync(resources).Result;

        public async Task<IEnumerable<IScript>> GetScriptsAsync(ScriptResources resources, CancellationToken token = default)
            => (await Task.WhenAll(languages.Select(s => s.GetScriptsAsync(resources, token)))).SelectMany(s => s);

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
