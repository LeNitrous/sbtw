// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages.Javascript.Scripts
{
    public class JavascriptScript : Script, IDisposable
    {
        protected readonly V8ScriptEngine Engine;
        private bool isDisposed;

        protected V8Script Compiled { get; set; }

        public JavascriptScript(V8ScriptEngine engine, string name, string path)
            : base(name, path)
        {
            Engine = engine;
            Engine.DocumentSettings.SearchPath = System.IO.Path.GetDirectoryName(path);
        }

        protected override void Perform()
        {
            Engine.Execute(Compiled);
        }

        protected override void Compile()
        {
            Compiled?.Dispose();
            Compiled = Engine.Compile(new DocumentInfo(Name) { Category = ModuleCategory.Standard }, System.IO.File.ReadAllText(Path));
        }

        protected override void RegisterMethod(string name, Delegate method)
            => Engine.AddHostObject(name, method);

        protected override void RegisterField(string name, object value)
            => Engine.Script[name] = value;

        protected override void RegisterType(Type type)
            => Engine.AddHostType(type);

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed && !disposing)
                return;

            Engine?.Dispose();
            Compiled?.Dispose();

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
