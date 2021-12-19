// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using Microsoft.ClearScript.V8;
using osu.Game.Database;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts.Javascript.Configuration;
using sbtw.Editor.Scripts.Javascript.Projects;

namespace sbtw.Editor.Scripts.Javascript
{
    public class JSEnvironment : ScriptEnvironment
    {
        public override string Name => "Javascript";

        protected new JSEnvironmentConfigManager ConfigManager => (JSEnvironmentConfigManager)base.ConfigManager;

        private readonly V8Runtime runtime;

        public JSEnvironment(RealmContextFactory realm, V8Runtime runtime)
            : base(realm)
        {
            this.runtime = runtime;
        }

        public override IScriptEnvironmentConfigManager CreateConfigManager(RealmContextFactory realm)
            => new JSEnvironmentConfigManager(this, realm);

        public override IProjectGenerator CreateGenerator()
            => new JSProjectGenerator();

        public override ScriptRuntime CreateRuntime()
        {
            var flag = V8ScriptEngineFlags.EnableDynamicModuleImports;
            int port = 0;

            if (ConfigManager.Get<bool>(JSEnvironmentSetting.DebuggingEnabled))
            {
                flag |= V8ScriptEngineFlags.EnableDebugging;
                port = ConfigManager.Get<int>(JSEnvironmentSetting.DebugPort);
            }

            return new JSScriptRuntime(runtime, flag, port);
        }
    }
}
