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
    public abstract class ScriptLanguage : IScriptLanguage
    {
        public bool IsDisposed { get; private set; }
        protected readonly IProject Project;

        public ScriptLanguage(IProject project)
        {
            Project = project;
        }

        public virtual string GetExceptionMessage(Exception exception)
            => exception.Message;

        public IEnumerable<IScript> GetScripts(Dictionary<string, object> resources = null)
            => GetScriptsAsync(resources).Result;

        public async Task<IEnumerable<IScript>> GetScriptsAsync(Dictionary<string, object> resources = null, CancellationToken token = default)
        {
            var scripts = await GetScriptsInternalAsync(resources, token);

            foreach (var script in scripts)
            {
                token.ThrowIfCancellationRequested();

                if (Project is ICanProvideAssets assets)
                    script.AssetProvider = assets;

                if (Project is ICanProvideGroups groups)
                    script.GroupProvider = groups;

                if (Project is ICanProvideFiles files)
                    script.FileProvider = files;

                if (Project is ICanProvideLogger logger)
                    script.Logger = logger;
            }

            return scripts;
        }

        protected abstract Task<IEnumerable<IScript>> GetScriptsInternalAsync(Dictionary<string, object> resources = null, CancellationToken token = default);

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
                IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public abstract class ScriptLanguage<T> : ScriptLanguage, IScriptLanguage<T>
        where T : IScript
    {
        protected ScriptLanguage(IProject project)
            : base(project)
        {
        }

        IEnumerable<T> IScriptLanguage<T>.GetScripts(Dictionary<string, object> resources)
            => ((IScriptLanguage<T>)this).GetScriptsAsync(resources).Result;

        async Task<IEnumerable<T>> IScriptLanguage<T>.GetScriptsAsync(Dictionary<string, object> resources, CancellationToken token)
            => (await GetScriptsAsync(resources, token)).Cast<T>();
    }
}
