// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections.Graphics;

namespace sbtw.Editor.Overlays.Settings
{
    public class GraphicsSection : SettingsSection
    {
        public override LocalisableString Header => "Graphics";

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.Laptop };

        public GraphicsSection()
        {
            Children = new Drawable[]
            {
                new RendererSettings(),
                new VideoSettings(),
            };
        }
    }
}
