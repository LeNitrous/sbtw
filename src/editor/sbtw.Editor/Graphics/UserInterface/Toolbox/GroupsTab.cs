// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Specialized;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Extensions;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osuTK;
using sbtw.Editor.Configuration;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public class GroupsTab : EditorTabbedToolboxTab
    {
        private BindableList<Group> groups;
        private GroupsList list;

        [BackgroundDependencyLoader]
        private void load(BindableList<Group> groups)
        {
            Child = list = new GroupsList { RelativeSizeAxes = Axes.Both };
            list.Items.BindCollectionChanged((_, args) => handleGroupsReorder(args));
            this.groups = groups.GetBoundCopy();
            this.groups.BindCollectionChanged((_, __) => Schedule(handleGroupsChange), true);
        }

        private void handleGroupsChange()
        {
            list.Items.Clear();
            list.Items.AddRange(groups);
        }

        private void handleGroupsReorder(NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Move:
                    groups.Move(args.OldStartingIndex, args.NewStartingIndex);
                    break;
            }
        }

        private class GroupsList : OsuRearrangeableListContainer<Group>
        {
            private Bindable<Group> selected;

            [BackgroundDependencyLoader]
            private void load(EditorSessionStatics statics)
            {
                selected = statics.GetBindable<Group>(EditorSessionStatic.GroupSelected);
                ScrollContainer.ScrollbarVisible = true;
            }

            private void selectAction(Group group)
                => selected.Value = group;

            protected override OsuRearrangeableListItem<Group> CreateOsuDrawable(Group item)
                => new GroupsListItem(item, selectAction);
        }

        private class GroupsListItem : OsuRearrangeableListItem<Group>
        {
            private readonly Action<Group> selectAction;

            public GroupsListItem(Group item, Action<Group> selectAction)
                : base(item)
            {
                this.selectAction = selectAction;
            }

            protected override Drawable CreateContent()
                => new GroupsListItemContent(Model, selectAction);
        }

        private class GroupsListItemContent : Container
        {
            public GroupsListItemContent(Group group, Action<Group> selectAction)
            {
                Height = 40;
                RelativeSizeAxes = Axes.X;

                Children = new Drawable[]
                {
                    new GroupLabel(group, selectAction)
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Direction = FillDirection.Horizontal,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Spacing = new Vector2(5, 0),
                        Children = new Drawable[]
                        {
                            new TargetSwitcher(group.Target) { Size = new Vector2(40), },
                            new VisibilityToggle(group.Visible) { Size = new Vector2(40), },
                        }
                    }
                };
            }

            private class GroupLabel : Container, IHasTooltip
            {
                public LocalisableString TooltipText
                    => $@"{"elements".ToQuantity(group.Elements.Count)} ({group.StartTime.ToEditorFormattedString()} -> {group.EndTime.ToEditorFormattedString()})";
                private readonly Group group;
                private readonly OsuSpriteText text;
                private readonly Action<Group> selectAction;

                public GroupLabel(Group group, Action<Group> selectAction)
                {
                    this.group = group;
                    this.selectAction = selectAction;
                    AutoSizeAxes = Axes.Both;
                    Padding = new MarginPadding { Left = 10 };
                    Children = new Drawable[]
                    {
                        text = new OsuSpriteText{ Text = group.Name },
                        new HoverClickSounds(HoverSampleSet.TabSelect),
                    };
                }

                protected override bool OnHover(HoverEvent e)
                {
                    text.FadeColour(Colour4.Yellow, 500, Easing.OutQuint);
                    return base.OnHover(e);
                }

                protected override void OnHoverLost(HoverLostEvent e)
                {
                    base.OnHoverLost(e);
                    text.FadeColour(Colour4.White, 500, Easing.OutQuint);
                }

                protected override bool OnClick(ClickEvent e)
                {
                    selectAction?.Invoke(group);
                    return base.OnClick(e);
                }
            }

            private class TargetSwitcher : IconButton
            {
                private readonly Bindable<ExportTarget> target;

                public TargetSwitcher(Bindable<ExportTarget> target)
                {
                    Action = cycle;
                    this.target = target.GetBoundCopy();
                    this.target.BindValueChanged(_ => handleTargetChange(), true);
                }

                private void cycle()
                    => target.Value = (ExportTarget)(((int)target.Value + 1 % 3 + 3) % 3);

                private void handleTargetChange()
                {
                    switch (target.Value)
                    {
                        case ExportTarget.Storyboard:
                            Icon = FontAwesome.Solid.Globe;
                            TooltipText = @"Export to .osb";
                            break;

                        case ExportTarget.Difficulty:
                            Icon = FontAwesome.Solid.FileAlt;
                            TooltipText = @"Export to .osu";
                            break;

                        case ExportTarget.None:
                            Icon = FontAwesome.Solid.TimesCircle;
                            TooltipText = @"Do not export";
                            break;
                    }
                }
            }

            private class VisibilityToggle : IconButton
            {
                private readonly Bindable<bool> visible;

                [Resolved]
                private Editor editor { get; set; }

                public VisibilityToggle(Bindable<bool> visible)
                {
                    Action = () => visible.Value = !visible.Value;
                    this.visible = visible.GetBoundCopy();
                    this.visible.BindValueChanged(_ => handleVisibilityChange(), true);
                }

                private void handleVisibilityChange()
                {
                    Icon = visible.Value ? FontAwesome.Solid.Eye : FontAwesome.Solid.EyeSlash;
                    TooltipText = visible.Value ? "Visible" : "Hidden";
                }
            }
        }
    }
}
