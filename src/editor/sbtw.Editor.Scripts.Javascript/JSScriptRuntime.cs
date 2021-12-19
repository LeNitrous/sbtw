// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using Microsoft.ClearScript.V8;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts.Javascript
{
    public class JSScriptRuntime : ScriptCompilingRuntime
    {
        static JSScriptRuntime()
        {
            V8Settings.EnableTopLevelAwait = true;
        }

        protected override string Extension => "js";
        private readonly V8Runtime runtime;
        private readonly V8ScriptEngineFlags flags;
        private readonly int debugPort;

        public JSScriptRuntime(V8Runtime runtime, V8ScriptEngineFlags flags, int debugPort)
        {
            this.flags = flags;
            this.runtime = runtime;
            this.debugPort = debugPort;
        }

        protected override CompilableScript CreateScript(Storage storage, string path)
            => new JSScript(CreateScriptEngine(), storage, path);

        protected V8ScriptEngine CreateScriptEngine() => runtime.CreateScriptEngine(flags, debugPort);

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed && !disposing)
                return;

            runtime.Dispose();

            base.Dispose(disposing);
        }
    }
}
