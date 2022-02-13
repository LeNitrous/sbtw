// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json.Linq;
using osu.Framework.IO.Stores;

namespace sbtw.Editor.Scripts.Javascript
{
    public class RuntimeUtilities : IDisposable
    {
        private readonly IResourceStore<byte[]> resources = new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(RuntimeUtilities).Assembly), "Resources");
        private readonly V8ScriptEngine engine;
        private bool isDisposed;

        public RuntimeUtilities(V8Runtime runtime)
            : this(runtime.CreateScriptEngine())
        {
        }

        public RuntimeUtilities(V8ScriptEngine engine = null)
        {
            this.engine = engine ?? new V8ScriptEngine();
            this.engine.Execute(readAllText("source-map.js"));
            this.engine.Execute(readAllText("typescript.js"));
        }

        public string GenerateSourceMap(string path, string code)
        {
            string filename = Path.GetFileName(path);
            string filepath = Path.GetDirectoryName(path);
            dynamic generator = engine.Evaluate($@"new sourceMap.SourceMapGenerator({{ file: ""{filename}"", sourceRoot: ""{filepath}"" }})");
            generator.setSourceContent(filename, code);

            string sourcemap = generator.toString();
            return Convert.ToBase64String(Encoding.Default.GetBytes(sourcemap));
        }

        public DocumentInfo Transpile(string path, string typescriptSource, out string javascriptSource)
        {
            dynamic ts = engine.Script.ts;
            dynamic option = engine.Script.JSON.parse(getTranspileOptions(path));
            dynamic output = ts.transpileModule(typescriptSource, option);

            var exceptions = new List<Exception>();

            foreach (var diagnostic in output.diagnostics)
            {
                var lineAndCharacter = ts.getLineAndCharacterOfPosition(diagnostic.file, diagnostic.start);
                var message = ts.flattenDiagnosticMessageText(diagnostic.messageText, "\n");
                exceptions.Add(new TypescriptException($"{diagnostic.file.fileName} ({lineAndCharacter.line + 1},{lineAndCharacter.character + 1}): {message}"));
            }

            if (exceptions.Count > 0)
                throw new AggregateException("One or more errors has has occured during transpiling.", exceptions);

            javascriptSource = output.outputText;

            return new DocumentInfo(new Uri(path))
            {
                SourceMapUri = new Uri(javascriptSource[javascriptSource.IndexOf("//# sourceMappingURL=")..].Replace("//# sourceMappingURL=", string.Empty)),
                Category = ModuleCategory.Standard,
            };
        }

        private string readAllText(string path)
            => Encoding.Default.GetString(resources.Get(path));

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
                isDisposed = true;

            engine.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static string getTranspileOptions(string path) => JObject.FromObject(new
        {
            fileName = Path.GetFileName(path),
            reportDiagnostics = true,
            compilerOptions = new
            {
                target = 99,
                allowJS = true,
                inlineSources = true,
                inlineSourceMap = true,
            },
        }).ToString().Replace(Environment.NewLine, string.Empty);
    }
}
