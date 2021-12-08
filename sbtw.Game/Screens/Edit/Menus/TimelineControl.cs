// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osu.Game.Beatmaps;
using osu.Game.Screens.Edit;
using osuTK;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class TimelineControl : BottomMenuBarItem
    {
        [Resolved]
        private EditorClock clock { get; set; }

        private readonly Box progress;
        private const float visualisation_size = 5.0f;

        public TimelineControl()
        {
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
                        new TimingVisualisationFlow(),
                        new KiaiVisualisationFlow(),
                        new BookmarksVisualisationFlow(),
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
                new SeekArea
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
            [Resolved]
            private EditorClock clock { get; set; }

            private ScheduledDelegate seekDelegate;

            protected override bool OnDragStart(DragStartEvent e) => true;

            protected override void OnDrag(DragEvent e) => seekToPosition(e.ScreenSpaceMousePosition);

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                seekToPosition(e.ScreenSpaceMousePosition);
                return base.OnMouseDown(e);
            }

            private void seekToPosition(Vector2 screenSpacePosition)
            {
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
            [BackgroundDependencyLoader]
            private void load(EditorBeatmap beatmap)
            {
                foreach (int bookmark in beatmap.BeatmapInfo.Bookmarks)
                    Add(new PointVisualisation(bookmark) { Colour = Colour4.Blue });
            }
        }

        private class KiaiVisualisationFlow : VisualisationFlow
        {
            [BackgroundDependencyLoader]
            private void load(EditorBeatmap beatmap, IBindable<WorkingBeatmap> working)
            {
                var points = beatmap.ControlPointInfo.EffectPoints;
                foreach (double startTime in points.Where(p => p.KiaiMode == true).Select(p => p.Time))
                {
                    double endTime = points.FirstOrDefault(p => p.Time > startTime && p.KiaiMode == false)?.Time ?? working.Value.Track.Length;
                    Add(new DurationVisualisation(startTime, endTime) { Colour = Colour4.Orange });
                }
            }
        }

        private class TimingVisualisationFlow : VisualisationFlow
        {
            [BackgroundDependencyLoader]
            private void load(EditorBeatmap beatmap)
            {
                foreach (double time in beatmap.ControlPointInfo.TimingPoints.Select(p => p.Time))
                    Add(new PointVisualisation(time) { Colour = Colour4.Red });
            }
        }
    }
}
