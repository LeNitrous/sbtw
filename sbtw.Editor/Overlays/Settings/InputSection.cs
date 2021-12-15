// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections.Input;

namespace sbtw.Editor.Overlays.Settings
{
    public class InputSection : SettingsSection
    {
        public override LocalisableString Header => @"Input";

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.MousePointer };

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            if (host.AvailableInputHandlers.FirstOrDefault(h => h is MouseHandler) is not MouseHandler mouseHandler)
                return;

            Children = new Drawable[]
            {
                new MouseSettings(mouseHandler),
            };
        }
    }
}
