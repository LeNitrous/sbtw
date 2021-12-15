// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ClearScript.V8;
using osu.Framework.Platform;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Scripts
{
    public class JSScriptRuntime : IScriptRuntime, IDisposable
    {
        private readonly V8Runtime runtime;
        private readonly List<JSScript> scripts = new List<JSScript>();
        private Storage storage;
        private bool isDisposed;

        static JSScriptRuntime()
        {
            V8Settings.EnableTopLevelAwait = true;
        }

        public JSScriptRuntime(EditorConfigManager config)
        {
            var port = config.Get<int>(EditorSetting.DebugPort);
            runtime = new V8Runtime("sbtw", V8RuntimeFlags.EnableDebugging | V8RuntimeFlags.EnableDynamicModuleImports, port);
        }

        public void Prepare(Storage storage)
        {
            if (this.storage != null)
                throw new InvalidOperationException($"There is currently a storage in-use. Use {nameof(Clear)} to make this runtime available for use again.");

            this.storage = storage;
        }

        public void Clear()
        {
            foreach (var script in scripts)
                script.Dispose();

            scripts.Clear();
            storage = null;
        }

        public IEnumerable<Script> Compile()
        {
            if (storage == null)
                throw new InvalidOperationException("Attempted to compile while there is no target storage.");

            if (isDisposed)
                throw new ObjectDisposedException("Attempted to compile when runtime is already disposed.");

            foreach (var path in storage.GetFiles(".", "*.js"))
            {
                if (scripts.Any(s => s.Path == path))
                    continue;

                scripts.Add(new JSScript(runtime.CreateScriptEngine(), storage, path));
            }

            foreach (var script in scripts)
                script.Compile();

            return scripts;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Clear();
                    runtime.Dispose();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
