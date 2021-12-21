// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using sbtw.Editor.Languages.Javascript.Resources;

namespace sbtw.Editor.Languages.Javascript.Scripts
{
    public class Typescript : IDisposable
    {
        private static readonly string typescriptCode;

        static Typescript()
        {
            using var stream = ResourceAssembly.GetStream("typescript.js");
            using var reader = new StreamReader(stream);
            typescriptCode = reader.ReadToEnd();
        }

        private readonly V8ScriptEngine engine;
        private readonly dynamic typescript;
        private readonly dynamic transpileOptions;
        private bool isDisposed;

        public Typescript(V8ScriptEngine engine)
        {
            this.engine = engine;
            this.engine.Execute(typescriptCode);
            this.engine.Execute("var opts = { compilerOptions: { inlineSourceMap: true, target: 99 }, reportDiagnostics: true }");

            typescript = this.engine.Script.ts;
            transpileOptions = this.engine.Script.opts;
        }

        public DocumentInfo Transpile(string path, string typescriptSource, out string javascriptSource)
        {
            dynamic output = typescript.transpileModule(typescriptSource, transpileOptions);

            if (output is Undefined)
                throw new Exception("Failed to transpile source");

            foreach (var diagnostic in output.diagnostics)
            {
                if (diagnostic.category == TypescriptDiagnosticCategory.Error)
                    throw new TypescriptException($"A typescript compiler error has occured");
            }

            javascriptSource = output.outputText;

            return new DocumentInfo(Path.GetFileName(path))
            {
                SourceMapUri = new Uri(javascriptSource[javascriptSource.IndexOf("//# sourceMappingURL=")..].Replace("//# sourceMappingURL=", string.Empty)),
                Category = ModuleCategory.Standard,
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed && !disposing)
                return;

            engine?.Dispose();

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
