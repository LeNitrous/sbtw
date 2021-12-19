// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using Microsoft.ClearScript.V8;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts.Javascript
{
    public class TSScriptRuntime : JSScriptRuntime
    {
        protected override string Extension => "ts";

        public TSScriptRuntime(V8Runtime runtime, V8ScriptEngineFlags flags, int debugPort)
            : base(runtime, flags, debugPort)
        {
        }

        protected override CompilableScript CreateScript(Storage storage, string path)
            => new TSScript(CreateScriptEngine(), storage, path);
    }
}
