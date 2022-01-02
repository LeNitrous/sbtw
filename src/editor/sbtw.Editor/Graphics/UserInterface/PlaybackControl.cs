// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Screens.Edit;
using osuTK;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class PlaybackControl : FillFlowContainer<PlaybackControlItem>
    {
        public PlaybackControl()
        {
            RelativeSizeAxes = Axes.X;
            Direction = FillDirection.Horizontal;
            Height = 80;
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;
            Padding = new MarginPadding(5);
            Spacing = new Vector2(5);
        }

        public void SetState(EditorBeatmap beatmap, EditorClock clock) => Schedule(() =>
        {
            clock ??= new EditorClock();
            beatmap ??= new EditorBeatmap(new Beatmap());

            Children = new PlaybackControlItem[]
            {
                new TimeInfo(clock),
                new SeekBar(beatmap, clock),
                new RateSelector(clock),
                new PlayButton(clock),
            };
        });
    }
}
