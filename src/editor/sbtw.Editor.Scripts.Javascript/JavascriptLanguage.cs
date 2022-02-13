// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts.Javascript
{
    public class JavascriptLanguage : FileBasedScriptLanguage<JavascriptScript>
    {
        public override IReadOnlyList<string> Extensions { get; } = new[] { ".ts", ".js" };
        public override IReadOnlyList<string> Exclude { get; } = new[] { "*.d.ts" };
        private readonly V8Runtime runtime;
        private readonly RuntimeUtilities utilities;

        public JavascriptLanguage(IProject project)
            : base(project)
        {
            var flags = V8RuntimeFlags.EnableDebugging
                | V8RuntimeFlags.EnableRemoteDebugging
                | V8RuntimeFlags.EnableDynamicModuleImports;

            runtime = new V8Runtime("sbtw", flags, 7270);
            runtime.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableAllLoading;
            utilities = new RuntimeUtilities(runtime);
        }

        protected override JavascriptScript CreateScript(string path)
        {
            var engine = runtime.CreateScriptEngine();
            engine.DocumentSettings.AccessFlags = runtime.DocumentSettings.AccessFlags;

            switch (Path.GetExtension(path))
            {
                case ".js":
                    return new JavascriptScript(engine, utilities, path);

                case ".ts":
                    return new TypescriptScript(engine, utilities, path);

                default:
                    throw new NotSupportedException("Unsupported file type.");
            }
        }

        protected override string GetExceptionMessage(Exception exception)
        {
            if (exception is ScriptEngineException sex)
            {
                if (sex.ScriptException != null && sex.ScriptException.constructor.name.Contains("Error"))
                    return sex.ScriptException.stack;

                return sex.ErrorDetails;
            }

            return base.GetExceptionMessage(exception);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            utilities.Dispose();
            runtime.Dispose();
        }
    }
}
