// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class BottomMenuBar : FillFlowContainer<BottomMenuBarItem>
    {
        public BottomMenuBar()
        {
            RelativeSizeAxes = Axes.X;
            Direction = FillDirection.Horizontal;
            Height = 80;
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;
            Padding = new MarginPadding(5);
            Spacing = new Vector2(5);
            Children = new BottomMenuBarItem[]
            {
                new TimeInfoContainer(),
                new TimelineControl(),
                new RateControl(),
                new PlaybackControl(),
            };
        }
    }
}
