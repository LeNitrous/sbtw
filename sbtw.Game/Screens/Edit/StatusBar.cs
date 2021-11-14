// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osuTK;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Edit
{
    public class StatusBar : Container
    {
        [BackgroundDependencyLoader]
        private void load(OsuColour colour)
        {
            Masking = true;
            CornerRadius = 5;
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colour.Gray1,
                },
                new FillFlowContainer<StatusBarIcon>
                {
                    Direction = FillDirection.Horizontal,
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Children = new StatusBarIcon[]
                    {
                        new DebuggerStatusIcon(),
                        new BuildingStatusIcon(),
                        new CleaningStatusIcon(),
                        new RestoringStatusIcon(),
                        new ProjectFileChangeStatusIcon(),
                        new BeatmapFileChangeStatusIcon(),
                    }
                }
            };
        }

        private abstract class StatusBarIcon : VisibilityContainer, IHasTooltip
        {
            public abstract LocalisableString TooltipText { get; }

            public abstract IconUsage Icon { get; }

            protected new Bindable<Visibility> State => base.State;

            [BackgroundDependencyLoader]
            private void load(OsuColour colour)
            {
                Size = new Vector2(30);
                Masking = true;
                CornerRadius = 5;
                Margin = new MarginPadding(5);
                Anchor = Anchor.CentreRight;
                Origin = Anchor.CentreRight;
                InternalChildren = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Icon = Icon,
                        Size = new Vector2(18),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = colour.Gray4,
                    },
                };
            }

            protected override void PopIn() => Alpha = 1;

            protected override void PopOut() => Alpha = 0;
        }

        private class DebuggerStatusIcon : StatusBarIcon
        {
            public override LocalisableString TooltipText => @"Debugger Connected";

            public override IconUsage Icon => FontAwesome.Solid.Link;

            [BackgroundDependencyLoader]
            private void load(DebuggerPoller poller)
            {
                poller.Attached.BindValueChanged(e => State.Value = e.NewValue ? Visibility.Visible : Visibility.Hidden, true);
            }
        }

        private abstract class NetProcessStatusIcon : StatusBarIcon
        {
            public abstract NetProcessStatus Status { get; }

            [BackgroundDependencyLoader]
            private void load(NetProcessListener listener)
            {
                listener.State.BindValueChanged(e => State.Value = (e.NewValue == Status) ? Visibility.Visible : Visibility.Hidden, true);
            }
        }

        private class BuildingStatusIcon : NetProcessStatusIcon
        {
            public override NetProcessStatus Status => NetProcessStatus.Building;

            public override LocalisableString TooltipText => @"Building";

            public override IconUsage Icon => FontAwesome.Solid.Hammer;
        }

        private class RestoringStatusIcon : NetProcessStatusIcon
        {
            public override NetProcessStatus Status => NetProcessStatus.Restoring;

            public override LocalisableString TooltipText => @"Restoring";

            public override IconUsage Icon => FontAwesome.Solid.Spinner;
        }

        private class CleaningStatusIcon : NetProcessStatusIcon
        {
            public override NetProcessStatus Status => NetProcessStatus.Cleaning;

            public override LocalisableString TooltipText => @"Cleaning";

            public override IconUsage Icon => FontAwesome.Solid.Broom;
        }

        private class ProjectFileChangeStatusIcon : StatusBarIcon
        {
            public override LocalisableString TooltipText => @"Scripts have been updated. Please regenerate.";

            public override IconUsage Icon => FontAwesome.Solid.Info;

            public virtual bool Condition(ProjectFileType type)
                => type == ProjectFileType.Script;

            [BackgroundDependencyLoader]
            private void load(Bindable<Project> project)
            {
                project.BindValueChanged(e =>
                {
                    if (e.OldValue != null)
                        e.OldValue.FileChanged -= handleFileEvent;

                    if (e.NewValue != null)
                        e.NewValue.FileChanged += handleFileEvent;
                }, true);
            }

            private void handleFileEvent(ProjectFileType type)
            {
                State.Value = Condition(type) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private class BeatmapFileChangeStatusIcon : ProjectFileChangeStatusIcon
        {
            public override LocalisableString TooltipText => @"Beatmap files have been changed. Please reload.";

            public override bool Condition(ProjectFileType type)
                => type == ProjectFileType.Beatmap || type == ProjectFileType.Storyboard || type == ProjectFileType.Resource;
        }
    }
}
