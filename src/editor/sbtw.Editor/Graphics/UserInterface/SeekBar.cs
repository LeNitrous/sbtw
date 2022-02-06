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
        private readonly Box progress;
        private const float visualisation_size = 5.0f;

        [Resolved]
        private Bindable<EditorClock> clock { get; set; }

        public SeekBar()
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
                new SeekArea()
                {
                    RelativeSizeAxes = Axes.Both
                },
            };
        }

        protected override void Update()
        {
            base.Update();
            progress.Width = (float)(clock.Value.CurrentTime / clock.Value.TrackLength);
        }

        private class SeekArea : Container
        {
            private ScheduledDelegate seekDelegate;

            [Resolved]
            private Bindable<EditorClock> clock { get; set; }

            protected override bool OnDragStart(DragStartEvent e) => true;

            protected override void OnDrag(DragEvent e) => seekToPosition(e.ScreenSpaceMousePosition);

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                seekToPosition(e.ScreenSpaceMousePosition);
                return base.OnMouseDown(e);
            }

            private void seekToPosition(Vector2 screenSpacePosition)
            {
                if (clock.Value.Track.Value is not TrackBass)
                    return;

                seekDelegate?.Cancel();
                seekDelegate = Schedule(() =>
                {
                    float pos = Math.Clamp(ToLocalSpace(screenSpacePosition).X, 0, DrawWidth);
                    clock.Value.SeekSmoothlyTo(pos / DrawWidth * clock.Value.TrackLength);
                });
            }
        }

        private abstract class VisualisationFlow : Container
        {
            private Bindable<EditorBeatmap> beatmap;
            private IBindable<WorkingBeatmap> working;

            [BackgroundDependencyLoader]
            private void load(Bindable<EditorBeatmap> beatmap, IBindable<WorkingBeatmap> working)
            {
                Height = visualisation_size;
                RelativeSizeAxes = Axes.X;

                this.beatmap = beatmap.GetBoundCopy();
                this.working = working.GetBoundCopy();

                this.beatmap.ValueChanged += _ => rebuildVisualisation();
                this.working.ValueChanged += _ => rebuildVisualisation();
                rebuildVisualisation();
            }

            protected abstract void CreateVisualisation(EditorBeatmap beatmap, WorkingBeatmap working);

            private void rebuildVisualisation() => Schedule(() =>
            {
                Clear();

                if (working.Value == null || beatmap.Value == null)
                    return;

                RelativeChildSize = new Vector2((float)Math.Max(1, working.Value.Track.Length), 1);
                CreateVisualisation(beatmap.Value, working.Value);
            });
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
            protected override void CreateVisualisation(EditorBeatmap beatmap, WorkingBeatmap working)
            {
                foreach (int bookmark in beatmap.BeatmapInfo.Bookmarks)
                    Add(new PointVisualisation(bookmark) { Colour = Colour4.Blue });
            }
        }

        private class KiaiVisualisationFlow : VisualisationFlow
        {
            protected override void CreateVisualisation(EditorBeatmap beatmap, WorkingBeatmap working)
            {
                var points = beatmap.ControlPointInfo.EffectPoints;
                foreach (double startTime in points.Where(p => p.KiaiMode == true).Select(p => p.Time))
                {
                    double endTime = points.FirstOrDefault(p => p.Time > startTime && p.KiaiMode == false)?.Time ?? working.Track.Length;
                    Add(new DurationVisualisation(startTime, endTime) { Colour = Colour4.Orange });
                }
            }
        }

        private class TimingVisualisationFlow : VisualisationFlow
        {
            protected override void CreateVisualisation(EditorBeatmap beatmap, WorkingBeatmap working)
            {
                foreach (double time in beatmap.ControlPointInfo.TimingPoints.Select(p => p.Time))
                    Add(new PointVisualisation(time) { Colour = Colour4.Red });
            }
        }
    }
}
