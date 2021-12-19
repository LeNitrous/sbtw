// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using sbtw.Editor.Configuration;
using sbtw.Editor.Studios;

namespace sbtw.Editor.Overlays.Settings
{
    public class GeneralSection : SettingsSection
    {
        public override LocalisableString Header => @"Editor";

        public override IEnumerable<string> FilterTerms => base.FilterTerms.Concat(new[] { "sbtw" });

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.Cog };

        public GeneralSection()
        {
            Children = new Drawable[]
            {
                new EnvironmentSettings(),
            };
        }

        private class EnvironmentSettings : SettingsSubsection
        {
            protected override LocalisableString Header => @"Environment";

            [Resolved]
            private EditorConfigManager config { get; set; }

            [Resolved]
            private StudioManager studioManager { get; set; }

            [BackgroundDependencyLoader]
            private void load()
            {
                Children = new Drawable[]
                {
                    new SettingsCheckbox
                    {
                        LabelText = @"Hot Reload",
                        Current = config.GetBindable<bool>(EditorSetting.HotReload),
                    },
                    new StudioSettingsDropdown
                    {
                        LabelText = @"Preferred Editor",
                        Current = studioManager.Current,
                        Items = studioManager.Studios,
                    }
                };
            }
        }

        private class StudioSettingsDropdown : SettingsDropdown<Studio>
        {
            protected override OsuDropdown<Studio> CreateDropdown() => new StudioDropdownControl();

            private class StudioDropdownControl : DropdownControl
            {
                protected override LocalisableString GenerateItemText(Studio item) => item.FriendlyName;
            }
        }
    }
}
