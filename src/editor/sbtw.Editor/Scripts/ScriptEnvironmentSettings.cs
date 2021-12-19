// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Overlays.Settings;

namespace sbtw.Editor.Scripts
{
    public abstract class ScriptEnvironmentSettings : SettingsSubsection
    {
        protected IScriptEnvironmentConfigManager ConfigManager { get; private set; }

        public ScriptEnvironmentSettings(ScriptEnvironment environment)
        {
            ConfigManager = environment.ConfigManager;
        }
    }
}
