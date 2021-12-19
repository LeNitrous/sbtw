// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Database;

namespace sbtw.Editor.Scripts.Javascript.Configuration
{
    public class JSEnvironmentConfigManager : ScriptEnvironmentConfigManager<JSEnvironmentSetting>
    {
        public JSEnvironmentConfigManager(ScriptEnvironment environment, RealmContextFactory context)
            : base(environment, context)
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(JSEnvironmentSetting.DebugPort, 7270);
            SetDefault(JSEnvironmentSetting.DebuggingEnabled, true);
        }
    }
}
