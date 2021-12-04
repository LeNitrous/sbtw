// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class BeatSnapControl : BottomMenuBarItem
    {
        public BeatSnapControl()
        {
            Width = 60;
            Child = new OsuSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = OsuFont.GetFont(size: 18, fixedWidth: true),
                Text = "1/4",
            };
        }
    }
}
