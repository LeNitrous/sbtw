// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Microsoft.ClearScript.V8;
using osu.Framework.Platform;
using sbtw.Editor.Scripts.Javascript.Resources;

namespace sbtw.Editor.Scripts.Javascript
{
    public class TSScript : JSScript
    {
        private V8Script compiled;
        private static dynamic transpile = null;

        public TSScript(V8ScriptEngine engine, Storage storage, string path)
            : base(engine, storage, path)
        {
            if (transpile != null)
                return;

            using var stream = ResourceAssembly.Resources.GetStream("typescriptServices.js");
            using var reader = new StreamReader(stream);
            engine.Execute(reader.ReadToEnd());

            transpile = engine.Evaluate("ts.transpile");
        }

        public override void Compile()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            using var stream = Storage.GetStream(Path);
            using var reader = new StreamReader(stream);

            string transpiled = transpile(reader.ReadToEnd());
            compiled = Engine.Compile(transpiled);
        }

        protected override void Perform()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (compiled == null)
                return;

            Engine.Execute(compiled);
        }
    }
}
