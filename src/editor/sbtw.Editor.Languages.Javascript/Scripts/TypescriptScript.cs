// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Microsoft.ClearScript.V8;

namespace sbtw.Editor.Languages.Javascript.Scripts
{
    public class TypescriptScript : JavascriptScript
    {
        private readonly Typescript typescript;

        public TypescriptScript(V8ScriptEngine engine, Typescript typescript, string name, string path)
            : base(engine, name, path)
        {
            this.typescript = typescript;
        }

        protected override void Perform()
            => Engine.Execute(typescript.Transpile(Path, File.ReadAllText(Path), out string source), source);
    }

    public class TypescriptException : Exception
    {
        public TypescriptException(string message)
            : base(message)
        {
        }
    }

    public enum TypescriptDiagnosticCategory
    {
        Warning,
        Error,
        Suggestion,
        Message,
    }
}
