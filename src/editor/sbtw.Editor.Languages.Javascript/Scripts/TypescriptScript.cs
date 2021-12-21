// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using Microsoft.ClearScript.V8;
using sbtw.Editor.Languages.Javascript.Resources;

namespace sbtw.Editor.Languages.Javascript.Scripts
{
    public class TypescriptScript : JavascriptScript
    {
        private static readonly string typescriptCode = readAllLines("typescript.js");

        public TypescriptScript(V8ScriptEngine engine, string name, string path)
            : base(engine, name, path)
        {
            engine.Execute("typescript.js", typescriptCode);
        }

        protected override void Compile()
        {
            Compiled?.Dispose();

            string source = File.ReadAllText(Path);
            dynamic transpileOptions = Engine.Evaluate("{ compilerOptions: {}, reportDiagnostics: true }");
            dynamic transpile = Engine.Evaluate("ts.transpile");

            dynamic output = transpile(source, transpileOptions);
            Compiled = Engine.Compile(System.IO.Path.GetFileName(Path), output.outputText);
        }

        private static string readAllLines(string path)
        {
            using var stream = ResourceAssembly.GetStream(path);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
