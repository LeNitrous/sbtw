// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using Microsoft.ClearScript.V8;
using sbtw.Common.Scripting;

namespace sbtw.Game.Scripting
{
    public class JSScript : ExternalScript
    {
        private readonly V8ScriptEngine engine;

        public JSScript(string path, V8ScriptEngine engine)
            : base(path)
        {
            this.engine = engine;
        }

        public override void Generate()
        {
            engine.AddHostObject("GetGroup", new Func<string, ScriptElementGroup>(GetGroup));
            engine.AddHostObject("SetVideo", new Action<string, int>(SetVideo));
            engine.ExecuteDocument(FilePath);
        }
    }
}
