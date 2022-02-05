// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using osu.Framework.Bindables;
using sbtw.Editor.Languages.Javascript.Projects;
using sbtw.Editor.Languages.Javascript.Scripts;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Languages.Javascript
{
    public class JavascriptLanguage : Language<JavascriptScript>
    {
        static JavascriptLanguage()
        {
            V8Settings.EnableTopLevelAwait = true;
        }

        public override string Name => @"Javascript";
        public override IEnumerable<string> Extensions { get; } = new[] { ".js", ".ts" };
        public override IEnumerable<string> Exclude { get; } = new[] { "sbtw.d.ts" };

        private V8Runtime runtime;
        private Typescript typescript;
        private readonly JavascriptConfigManager config;
        private readonly Bindable<int> debugPort;
        private readonly Bindable<bool> debugEnabled;

        public JavascriptLanguage()
        {
            config = new JavascriptConfigManager();
            debugPort = config.GetBindable<int>(JavascriptSetting.DebugPort);
            debugEnabled = config.GetBindable<bool>(JavascriptSetting.DebugEnabled);

            debugPort.ValueChanged += _ => createRuntime();
            debugEnabled.ValueChanged += _ => createRuntime();

            createRuntime();
        }

        private void createRuntime()
        {
            runtime?.Dispose();
            typescript?.Dispose();

            var flags = V8RuntimeFlags.EnableDynamicModuleImports;
            //if (debugEnabled.Value)
            flags |= V8RuntimeFlags.EnableDebugging | V8RuntimeFlags.EnableRemoteDebugging;

            runtime = new V8Runtime("sbtw", flags, debugPort.Value);
            runtime.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableAllLoading;

            typescript = new Typescript(runtime.CreateScriptEngine());
        }

        public override IProjectGenerator CreateProjectGenerator() => new JavascriptProjectGenerator();
        public override ILanguageConfigManager CreateConfigManager() => config;

        public override string GetExceptionMessage(Exception exception)
        {
            if (exception is ScriptEngineException sex)
            {
                if (sex.ScriptException != null && sex.ScriptException.constructor.name == "Error")
                    return sex.ScriptException.stack;

                return sex.ErrorDetails;
            }

            return base.GetExceptionMessage(exception);
        }

        protected override JavascriptScript CreateScript(string name, string path)
        {
            var engine = runtime.CreateScriptEngine();
            engine.DocumentSettings.AccessFlags = runtime.DocumentSettings.AccessFlags;
            engine.DocumentSettings.Loader = new TypescriptDocumentLoader();

            switch (Path.GetExtension(path))
            {
                case ".js":
                    return new JavascriptScript(engine, name, path);

                case ".ts":
                    return new TypescriptScript(engine, typescript, name, path);

                default:
                    throw new ArgumentException(@"Cannot create a script for an unknown file type.");
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            typescript?.Dispose();
            runtime?.Dispose();
            config?.Dispose();
        }

        private class TypescriptDocumentLoader : DocumentLoader
        {
            public override async Task<Document> LoadDocumentAsync(DocumentSettings settings, DocumentInfo? sourceInfo, string specifier, DocumentCategory category, DocumentContextCallback contextCallback)
            {
                Default.DiscardCachedDocuments();
                var document = await Default.LoadDocumentAsync(settings, sourceInfo, specifier, category, contextCallback);

                using var reader = new StreamReader(document.Contents);
                using var typescript = new Typescript(new V8ScriptEngine());
                var transpiled = typescript.Transpile(document.Info.Uri.AbsolutePath, await reader.ReadToEndAsync(), out string source);

                return new StringDocument(transpiled, source);
            }
        }
    }
}
