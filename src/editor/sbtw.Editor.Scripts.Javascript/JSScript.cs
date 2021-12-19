// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using Microsoft.ClearScript.V8;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts.Javascript
{
    public class JSScript : CompilableScript
    {
        protected readonly V8ScriptEngine Engine;
        private V8Script compiled;

        public JSScript(V8ScriptEngine engine, Storage storage, string path)
            : base(storage, path)
        {
            Engine = engine;
            Engine.AddHostObject("GetValue", new Func<string, object>(GetValue));
            Engine.AddHostObject("SetValue", new Func<string, object, object>(SetValue));
            Engine.AddHostObject("SetVideo", new Action<string, int>(SetVideo));
            Engine.AddHostObject("OpenFile", new Func<string, byte[]>(OpenFile));
            Engine.AddHostObject("GetGroup", new Func<string, ScriptElementGroup>(GetGroup));
        }

        public override void Compile()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            compiled?.Dispose();
            compiled = Engine.CompileDocument(Path);
        }

        protected override void Perform()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (compiled == null)
                return;

            Engine.Execute(compiled);
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed && !disposing)
                return;

            Engine.Dispose();
            compiled?.Dispose();

            base.Dispose(disposing);
        }
    }
}
