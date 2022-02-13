// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace sbtw.Editor.Graphics.UserInterface.Bottom
{
    public class BottomControlItem : Container
    {
        protected override Container<Drawable> Content { get; }

        public BottomControlItem()
        {
            Height = 40;
            Masking = true;
            CornerRadius = 5;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Colour = Colour4.FromHex("111"),
                    RelativeSizeAxes = Axes.Both,
                },
                Content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(5),
                },
            };
        }
    }
}
