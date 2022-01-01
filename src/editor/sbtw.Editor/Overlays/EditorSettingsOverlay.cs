// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections;

namespace sbtw.Editor.Overlays
{
    public class EditorSettingsOverlay : SettingsPanel
    {
        public EditorSettingsOverlay()
            : base(true)
        {
        }

        protected override Drawable CreateHeader() => new SettingsHeader("settings", "change how sbtw! behaves");

        protected override IEnumerable<SettingsSection> CreateSections() => new SettingsSection[]
        {
            new Settings.GeneralSection(),
            new Settings.InputSection(),
            new AudioSection(),
            new Settings.GraphicsSection(),
            new Settings.DebugSection(),
        };
    }
}
