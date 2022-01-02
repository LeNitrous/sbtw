// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osu.Game.Beatmaps;
using osu.Game.Screens.Edit;
using osuTK;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class SeekBar : PlaybackControlItem
    {
        private readonly EditorClock clock;
        private readonly Box progress;
        private const float visualisation_size = 5.0f;

        public SeekBar(EditorBeatmap beatmap, EditorClock clock)
        {
            this.clock = clock;

            Width = 600;
            Content.Padding = new MarginPadding { Horizontal = 10 };
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.BottomLeft,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new TimingVisualisationFlow(beatmap),
                        new KiaiVisualisationFlow(beatmap),
                        new BookmarksVisualisationFlow(beatmap),
                    }
                },
                progress = new Box
                {
                    Height = 2,
                    Colour = Colour4.DarkGray,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.X,
                },
                new SeekArea(clock)
                {
                    RelativeSizeAxes = Axes.Both
                },
            };
        }

        protected override void Update()
        {
            base.Update();
            progress.Width = (float)(clock.CurrentTime / clock.TrackLength);
        }

        private class SeekArea : Container
        {
            private readonly EditorClock clock;
            private ScheduledDelegate seekDelegate;

            public SeekArea(EditorClock clock)
            {
                this.clock = clock;
            }

            protected override bool OnDragStart(DragStartEvent e) => true;

            protected override void OnDrag(DragEvent e) => seekToPosition(e.ScreenSpaceMousePosition);

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                seekToPosition(e.ScreenSpaceMousePosition);
                return base.OnMouseDown(e);
            }

            private void seekToPosition(Vector2 screenSpacePosition)
            {
                if (clock.Track.Value is not TrackBass)
                    return;

                seekDelegate?.Cancel();
                seekDelegate = Schedule(() =>
                {
                    float pos = Math.Clamp(ToLocalSpace(screenSpacePosition).X, 0, DrawWidth);
                    clock.SeekSmoothlyTo(pos / DrawWidth * clock.TrackLength);
                });
            }
        }

        private class VisualisationFlow : Container
        {
            protected readonly EditorBeatmap Beatmap;

            public VisualisationFlow(EditorBeatmap beatmap)
            {
                Beatmap = beatmap;
            }

            [BackgroundDependencyLoader]
            private void load(IBindable<WorkingBeatmap> working)
            {
                Height = visualisation_size;
                RelativeSizeAxes = Axes.X;
                RelativeChildSize = new Vector2((float)Math.Max(1, working.Value.Track.Length), 1);
            }
        }

        private class PointVisualisation : Circle
        {
            public PointVisualisation(double startTime)
            {
                X = (float)startTime;
                Size = new Vector2(visualisation_size);
                RelativePositionAxes = Axes.X;
            }
        }

        private class DurationVisualisation : Circle
        {
            public DurationVisualisation(double startTime, double endTime)
            {
                X = (float)startTime;
                Width = (float)(endTime - startTime);
                Height = visualisation_size;
                RelativeSizeAxes = Axes.X;
                RelativePositionAxes = Axes.X;
            }
        }

        private class BookmarksVisualisationFlow : VisualisationFlow
        {
            public BookmarksVisualisationFlow(EditorBeatmap beatmap)
                : base(beatmap)
            {
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                foreach (int bookmark in Beatmap.BeatmapInfo.Bookmarks)
                    Add(new PointVisualisation(bookmark) { Colour = Colour4.Blue });
            }
        }

        private class KiaiVisualisationFlow : VisualisationFlow
        {
            public KiaiVisualisationFlow(EditorBeatmap beatmap)
                : base(beatmap)
            {
            }

            [BackgroundDependencyLoader]
            private void load(IBindable<WorkingBeatmap> working)
            {
                var points = Beatmap.ControlPointInfo.EffectPoints;
                foreach (double startTime in points.Where(p => p.KiaiMode == true).Select(p => p.Time))
                {
                    double endTime = points.FirstOrDefault(p => p.Time > startTime && p.KiaiMode == false)?.Time ?? working.Value.Track.Length;
                    Add(new DurationVisualisation(startTime, endTime) { Colour = Colour4.Orange });
                }
            }
        }

        private class TimingVisualisationFlow : VisualisationFlow
        {
            public TimingVisualisationFlow(EditorBeatmap beatmap)
                : base(beatmap)
            {
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                foreach (double time in Beatmap.ControlPointInfo.TimingPoints.Select(p => p.Time))
                    Add(new PointVisualisation(time) { Colour = Colour4.Red });
            }
        }
    }
}
