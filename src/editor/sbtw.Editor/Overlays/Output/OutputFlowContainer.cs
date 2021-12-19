// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Graphics.Containers;
using osu.Game.Online.Chat;
using osu.Game.Overlays.Chat;

namespace sbtw.Editor.Overlays.Output
{
    public class OutputFlowContainer : Container
    {
        private FillFlowContainer flow;

        protected override Container<Drawable> Content => flow;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new OutputScrollContainer
            {
                ScrollbarVisible = true,
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Bottom = 5 },
                Child = flow = new FillFlowContainer
                {
                    Padding = new MarginPadding { Horizontal = 20 },
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                }
            };
        }

        public void AddLine(Message message) => Add(new ChatLine(message));

        public void AddSeparator(DateTimeOffset date) => Add(new DrawableChannel.DaySeparator(date) { Alpha = 0.1f });

        private class OutputScrollContainer : UserTrackingScrollContainer
        {
            private float? lastExtent;

            protected override void OnUserScroll(float value, bool animated = true, double? distanceDecay = null)
            {
                base.OnUserScroll(value, animated, distanceDecay);
                lastExtent = null;
            }

            protected override void Update()
            {
                base.Update();

                if (UserScrolling && IsScrolledToEnd(10))
                    CancelUserScroll();

                bool requiresScrollUpdate = !UserScrolling && (lastExtent == null || Precision.AlmostBigger(ScrollableExtent, lastExtent.Value));

                if (!requiresScrollUpdate)
                    return;

                Schedule(() =>
                {
                    if (UserScrolling)
                        return;

                    if (Current < ScrollableExtent)
                        ScrollToEnd();

                    lastExtent = ScrollableExtent;
                });
            }
        }
    }
}
