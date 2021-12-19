// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Game.Database;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    public abstract class ScriptEnvironment : IDisposable
    {
        public abstract string Name { get; }

        public IScriptEnvironmentConfigManager ConfigManager { get; protected set; }
        public readonly ScriptRuntime Runtime;
        private bool isDisposable;

        public ScriptEnvironment(RealmContextFactory realm)
        {
            ConfigManager = CreateConfigManager(realm);
            Runtime = CreateRuntime();
        }

        public virtual ScriptEnvironmentSettings CreateSettings() => null;
        public virtual IProjectGenerator CreateGenerator() => null;
        public abstract ScriptRuntime CreateRuntime();
        public abstract IScriptEnvironmentConfigManager CreateConfigManager(RealmContextFactory realm);

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposable && !disposing)
                return;

            ConfigManager.Dispose();
            Runtime.Dispose();

            isDisposable = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
