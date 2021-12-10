// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Numerics;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using sbtw.Common.Scripting;

namespace sbtw.Game.Scripting
{
    public class JSScript : ExternalScript
    {
        public JSScript(string path)
            : base(path)
        {
        }

        public override void Generate()
        {
            using var engine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDebugging | V8ScriptEngineFlags.EnableRemoteDebugging | V8ScriptEngineFlags.EnableDynamicModuleImports);
            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
            engine.AddHostObject("GetGroup", new Func<string, ScriptElementGroup>(GetGroup));
            engine.AddHostObject("SetVideo", new Action<string, int>(SetVideo));
            engine.AddHostType(typeof(Layer));
            engine.AddHostType(typeof(Color));
            engine.AddHostType(typeof(Anchor));
            engine.AddHostType(typeof(Vector2));
            engine.AddHostType(typeof(LoopType));
            engine.ExecuteDocument(FilePath);
        }
    }
}
