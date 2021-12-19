// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections.DebugSettings;

namespace sbtw.Editor.Overlays.Settings
{
    public class DebugSection : SettingsSection
    {
        public override LocalisableString Header => @"Debug";

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.Bug };

        public DebugSection()
        {
            Children = new Drawable[]
            {
                new DebugSettings(),
                new MemorySettings(),
            };
        }

        private class DebugSettings : SettingsSubsection
        {
            protected override LocalisableString Header => @"Developer";

            [BackgroundDependencyLoader(true)]
            private void load(FrameworkDebugConfigManager config, FrameworkConfigManager frameworkConfig)
            {
                Children = new Drawable[]
                {
                    new SettingsCheckbox
                    {
                        LabelText = DebugSettingsStrings.ShowLogOverlay,
                        Current = frameworkConfig.GetBindable<bool>(FrameworkSetting.ShowLogOverlay)
                    },
                    new SettingsCheckbox
                    {
                        LabelText = DebugSettingsStrings.BypassFrontToBackPass,
                        Current = config.GetBindable<bool>(DebugSetting.BypassFrontToBackPass)
                    }
                };
            }
        }
    }
}
