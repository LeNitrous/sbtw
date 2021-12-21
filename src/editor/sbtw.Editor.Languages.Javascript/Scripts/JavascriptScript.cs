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
        public override string Name { get; }

        protected readonly V8ScriptEngine Engine;
        protected readonly string Path;
        private bool isDisposed;

        protected V8Script Compiled { get; set; }

        public JavascriptScript(V8ScriptEngine engine, string name, string path)
        {
            Name = name;
            Path = path;
            Engine = engine;
            Engine.AddHostObject("GetValue", new Func<string, object>(GetValue));
            Engine.AddHostObject("SetValue", new Func<string, object, object>(SetValue));
            Engine.AddHostObject("SetVideo", new Action<string, int>(SetVideo));
            Engine.AddHostObject("GetGroup", new Func<string, ScriptElementGroup>(GetGroup));
            Engine.DocumentSettings.SearchPath = System.IO.Path.GetDirectoryName(path);
        }

        protected override void Perform()
        {
            Engine.Execute(Compiled);
        }

        protected override void Compile()
        {
            Compiled?.Dispose();
            // Compiled = Engine.CompileDocument(Path, ModuleCategory.Standard);
            Compiled = Engine.Compile(new DocumentInfo(Name)
            {
                Category = ModuleCategory.Standard,
            }, System.IO.File.ReadAllText(Path));
        }

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
