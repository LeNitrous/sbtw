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
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Configuration;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Graphics.UserInterface.Bottom
{
    public class SeekControl : BottomControlItem
    {
        private const float visualisation_size = 5.0f;
        private Box progress;

        [Resolved]
        private EditorClock clock { get; set; }

        public SeekControl()
        {
            Width = 600;
            Content.Padding = new MarginPadding { Horizontal = 10 };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
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
                        new FlowTimingPoints(),
                        new FlowKiai(),
                        new FlowBookmarks(),
                    }
                },
                new FlowGroupSelected
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0.2f,
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
            progress.Width = clock.TrackLength > 0 ? (float)(clock.CurrentTime / clock.TrackLength) : 0;
        }

        private class SeekArea : Container
        {
            private ScheduledDelegate seekDelegate;

            [Resolved]
            private EditorClock clock { get; set; }

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
                    clock.Seek(pos / DrawWidth * clock.TrackLength);
                });
            }
        }

        private abstract class VisualisationFlow : Container
        {
            private IBindable<WorkingBeatmap> working;

            [Resolved]
            private EditorBase editor { get; set; }

            [Resolved]
            private IBeatmapProvider beatmapProvider { get; set; }

            public VisualisationFlow()
            {
                Height = visualisation_size;
            }

            [BackgroundDependencyLoader]
            private void load(IBindable<WorkingBeatmap> working)
            {
                RelativeSizeAxes = Axes.X;

                this.working = working.GetBoundCopy();
                this.working.BindValueChanged(_ => rebuildVisualisation(), true);
            }

            protected abstract void CreateVisualisation(IBeatmap beatmap, WorkingBeatmap working);

            private void rebuildVisualisation() => Schedule(() =>
            {
                Clear();

                if (working.Value == null)
                    return;

                var beatmap = beatmapProvider.GetBeatmap(working.Value.BeatmapInfo) ?? new Beatmap();

                RelativeChildSize = new Vector2((float)Math.Max(1, working.Value.Track.Length), 1);
                CreateVisualisation(beatmap, working.Value);
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

        private class FlowBookmarks : VisualisationFlow
        {
            protected override void CreateVisualisation(IBeatmap beatmap, WorkingBeatmap working)
            {
                foreach (int bookmark in beatmap.BeatmapInfo.Bookmarks)
                    Add(new PointVisualisation(bookmark) { Colour = Colour4.Blue });
            }
        }

        private class FlowKiai : VisualisationFlow
        {
            protected override void CreateVisualisation(IBeatmap beatmap, WorkingBeatmap working)
            {
                var points = beatmap.ControlPointInfo.EffectPoints;
                foreach (double startTime in points.Where(p => p.KiaiMode == true).Select(p => p.Time))
                {
                    double endTime = points.FirstOrDefault(p => p.Time > startTime && p.KiaiMode == false)?.Time ?? working.Track.Length;
                    Add(new DurationVisualisation(startTime, endTime) { Colour = Colour4.Orange });
                }
            }
        }

        private class FlowTimingPoints : VisualisationFlow
        {
            protected override void CreateVisualisation(IBeatmap beatmap, WorkingBeatmap working)
            {
                foreach (double time in beatmap.ControlPointInfo.TimingPoints.Select(p => p.Time))
                    Add(new PointVisualisation(time) { Colour = Colour4.Red });
            }
        }

        private class FlowGroupSelected : VisualisationFlow
        {
            private IBindable<Group> selected;

            [BackgroundDependencyLoader]
            private void load(EditorSessionStatics statics)
            {
                AutoSizeAxes = Axes.Y;
                selected = statics.GetBindable<Group>(EditorSessionStatic.GroupSelected);
                selected.ValueChanged += e => handleGroupChange(e.NewValue);
            }

            private void handleGroupChange(Group group)
            {
                Clear();

                if (group == null)
                    return;

                Add(new DurationVisualisation(group.StartTime, group.EndTime) { Height = 10 });
            }

            protected override void CreateVisualisation(IBeatmap beatmap, WorkingBeatmap working)
            {
            }
        }
    }
}
