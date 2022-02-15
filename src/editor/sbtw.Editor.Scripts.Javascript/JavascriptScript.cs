// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace sbtw.Editor.Scripts.Javascript
{
    public class JavascriptScript : FileBasedScript
    {
        protected readonly V8ScriptEngine Engine;
        protected readonly RuntimeUtilities Utilities;
        private V8Script compiled;

        public JavascriptScript(V8ScriptEngine engine, RuntimeUtilities utilities, string path)
            : base(path)
        {
            Engine = engine;
            Engine.DocumentSettings.SearchPath = System.IO.Path.GetDirectoryName(path);
            Utilities = utilities;
        }

        public sealed override Task CompileAsync(CancellationToken token = default)
        {
            compiled?.Dispose();
            compiled = Engine.Compile(GetDocumentInfo(out string code), code);
            return Task.CompletedTask;
        }

        public sealed override Task ExecuteAsync(CancellationToken token = default)
        {
            Engine.Execute(compiled);
            return Task.CompletedTask;
        }

        protected virtual DocumentInfo GetDocumentInfo(out string code)
        {
            code = System.IO.File.ReadAllText(Path);
            string sourcemap = Utilities.GenerateSourceMap(Path, code);
            return new DocumentInfo(new Uri(Path))
            {
                SourceMapUri = new Uri($"data:application/json;base64,{sourcemap}"),
                Category = ModuleCategory.Standard
            };
        }

        protected sealed override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Engine.Dispose();
            compiled?.Dispose();
        }

        public sealed override void RegisterFunction(Delegate del)
            => Engine.AddHostObject(del.Method.Name, del);

        public sealed override void RegisterType(Type type)
            => Engine.AddHostType(type);

        public sealed override void RegisterVariable(string name, object value)
            => Engine.Script[name] = value;
    }
}
