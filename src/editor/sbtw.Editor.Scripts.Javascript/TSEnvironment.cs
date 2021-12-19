// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using Microsoft.ClearScript.V8;
using osu.Game.Database;
using sbtw.Editor.Scripts.Javascript.Configuration;

namespace sbtw.Editor.Scripts.Javascript
{
    public class TSEnvironment : JSEnvironment
    {
        private readonly V8Runtime runtime;

        public TSEnvironment(RealmContextFactory realm, V8Runtime runtime)
            : base(realm, runtime)
        {
            this.runtime = runtime;
        }

        public override ScriptRuntime CreateRuntime()
            => new TSScriptRuntime(runtime, V8ScriptEngineFlags.EnableDebugging | V8ScriptEngineFlags.EnableDynamicModuleImports, ConfigManager.Get<int>(JSEnvironmentSetting.DebugPort));
    }
}
