// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osuTK;
using sbtw.Editor.Overlays.Setup;

namespace sbtw.Editor.Overlays
{
    public class SetupOverlay : OsuFocusedOverlayContainer
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        public SetupOverlay()
        {
            Size = new Vector2(900, 470);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colour)
        {
            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 10,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = colourProvider.Background3,
                        RelativeSizeAxes = Axes.Both,
                    },
                    new SectionsContainer<SetupSection>
                    {
                        Padding = new MarginPadding { Bottom = 60 },
                        RelativeSizeAxes = Axes.Both,
                        FixedHeader = new SetupHeader { Margin = new MarginPadding { Bottom = 5 } },
                        ChildrenEnumerable = new SetupSection[]
                        {
                            new SetupSectionProject(),
                            new SetupSectionBeatmap(),
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                Colour = colourProvider.Dark5,
                                RelativeSizeAxes = Axes.Both,
                            },
                            new OsuButton
                            {
                                Text = "Create",
                                Width = 200,
                                Margin = new MarginPadding { Vertical = 10, Right = 10 },
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                BackgroundColour = colour.Yellow,
                            },
                        }
                    }
                },
            };
        }
    }
}
