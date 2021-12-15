// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ClearScript.V8;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts
{
    public class JSScriptRuntime : IScriptRuntime, IDisposable
    {
        public Storage Storage { get; set; }

        private readonly V8Runtime runtime = new V8Runtime("sbtw", V8RuntimeFlags.EnableDebugging | V8RuntimeFlags.EnableDynamicModuleImports, 7270);
        private readonly List<JSScript> scripts = new List<JSScript>();
        private bool isDisposed;

        static JSScriptRuntime()
        {
            V8Settings.EnableTopLevelAwait = true;
        }

        public void Clear()
        {
            foreach (var script in scripts)
                script.Dispose();

            scripts.Clear();
        }

        public IEnumerable<Script> Compile()
        {
            if (Storage == null)
                throw new InvalidOperationException("Attempted to compile while there is no target storage.");

            if (isDisposed)
                throw new ObjectDisposedException("Attempted to compile when runtime is already disposed.");

            foreach (var path in Storage.GetFiles(".", "*.js"))
            {
                if (scripts.Any(s => s.Path == path))
                    continue;

                scripts.Add(new JSScript(runtime.CreateScriptEngine(), Storage, path));
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
