// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ClearScript.V8;
using osu.Framework.Bindables;
using osu.Game.Database;
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
        public override IEnumerable<string> Extensions { get; } = new[] { "js", "ts" };

        private V8Runtime runtime;
        private readonly JavascriptConfigManager config;
        private readonly Bindable<int> debugPort;
        private readonly Bindable<bool> debugEnabled;

        public JavascriptLanguage(RealmContextFactory realm)
        {
            config = new JavascriptConfigManager(this, realm);
            debugPort = config.GetBindable<int>(JavascriptSetting.DebugPort);
            debugEnabled = config.GetBindable<bool>(Javascript.JavascriptSetting.DebugEnabled);

            debugPort.ValueChanged += _ => createRuntime();
            debugEnabled.ValueChanged += _ => createRuntime();

            createRuntime();
        }

        private void createRuntime()
        {
            Clear();
            runtime?.Dispose();

            var flags = V8RuntimeFlags.EnableDynamicModuleImports;
            if (debugEnabled.Value)
                flags |= V8RuntimeFlags.EnableDebugging;

            runtime = new V8Runtime("sbtw", flags, debugPort.Value);
        }

        public override IProjectGenerator CreateProjectGenerator() => new JavascriptProjectGenerator();
        public override ILanguageConfigManager CreateConfigManager(RealmContextFactory realm) => config;

        protected override JavascriptScript CreateScript(string name, string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".js":
                    return new JavascriptScript(runtime.CreateScriptEngine(), name, path);

                case ".ts":
                    return new TypescriptScript(runtime.CreateScriptEngine(), name, path);

                default:
                    throw new ArgumentException(@"Cannot create a script for an unknown file type.");
            }
        }

        protected override void Clear()
        {
            foreach (var script in Cache)
                script.Dispose();

            base.Clear();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            runtime?.Dispose();
            config?.Dispose();
        }
    }
}
