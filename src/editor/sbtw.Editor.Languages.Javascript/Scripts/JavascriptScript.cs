// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages.Javascript.Scripts
{
    public class JavascriptScript : Script
    {
        protected V8ScriptEngine Engine { get; private set; }

        public JavascriptScript(V8ScriptEngine engine, string name, string path)
            : base(name, path)
        {
            Engine = engine;
            Engine.DocumentSettings.SearchPath = System.IO.Path.GetDirectoryName(path);
        }

        protected override void Perform()
            => Engine.Evaluate(new DocumentInfo(new Uri(Path)) { Category = ModuleCategory.Standard }, System.IO.File.ReadAllText(Path)).ToTask().Wait();

        protected override void RegisterMethod(string name, Delegate method) => Engine.AddHostObject(name, method);
        protected override void RegisterField(string name, object value) => Engine.Script[name] = value;
        protected override void RegisterType(Type type) => Engine.AddHostType(type);

        protected override string FormatException(Exception ex)
        {
            if (ex is ScriptEngineException sex)
                return sex.ErrorDetails;

            return base.FormatException(ex);
        }

        protected override void Dispose(bool disposing)
        {
            Engine?.Dispose();
            Engine = null;
            base.Dispose(disposing);
        }
    }
}
