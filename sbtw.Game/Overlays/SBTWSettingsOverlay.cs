// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections;

namespace sbtw.Game.Overlays
{
    public class SBTWSettingsOverlay : SettingsPanel
    {
        public SBTWSettingsOverlay()
            : base(true)
        {
        }

        protected override Drawable CreateHeader() => new SettingsHeader("settings", "change how sbtw! behaves");

        protected override IEnumerable<SettingsSection> CreateSections() => new SettingsSection[]
        {
            new RulesetSection(),
            new AudioSection(),
            new GraphicsSection(),
        };
    }
}
