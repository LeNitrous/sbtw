// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osuTK;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Graphics.UserInterface
{
    public partial class Toolbox
    {
        private class GroupsToolboxTab : Container
        {
            private BindableList<GroupSetting> groups;
            private Bindable<IProject> project;
            private GroupsList list;

            [BackgroundDependencyLoader]
            private void load(Bindable<IProject> project)
            {
                RelativeSizeAxes = Axes.Both;
                Child = list = new GroupsList
                {
                    RelativeSizeAxes = Axes.Both,
                };

                this.project = project.GetBoundCopy();
                this.project.BindValueChanged(e => Schedule(() =>
                {
                    groups = e.NewValue.Groups?.GetBoundCopy();
                    list.Items.UnbindBindings();

                    if (groups != null)
                        list.Items.BindTo(groups);
                }), true);
            }

            private class GroupsList : OsuRearrangeableListContainer<GroupSetting>
            {
                protected override OsuRearrangeableListItem<GroupSetting> CreateOsuDrawable(GroupSetting item)
                    => new GroupsListItem(item);
            }

            private class GroupsListItem : OsuRearrangeableListItem<GroupSetting>
            {
                public GroupsListItem(GroupSetting item)
                    : base(item)
                {
                }

                protected override Drawable CreateContent() => new GroupsListItemContent(Model);
            }

            private class GroupsListItemContent : Container
            {
                public GroupsListItemContent(GroupSetting model)
                {
                    Height = 40;
                    RelativeSizeAxes = Axes.X;
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Text = model.Name,
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
                                new TargetIconButton(model.Target) { Size = new Vector2(40), },
                                new HiddenToggleButton(model.Hidden) { Size = new Vector2(40), },
                            }
                        }
                    };


                }

                private class TargetIconButton : IconButton
                {
                    private readonly Bindable<ExportTarget> target;

                    public TargetIconButton(Bindable<ExportTarget> target)
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
                                Icon = FontAwesome.Solid.MinusCircle;
                                TooltipText = @"Do not export";
                                break;
                        }
                    }
                }

                private class HiddenToggleButton : IconButton
                {
                    private readonly Bindable<bool> hidden;

                    [Resolved]
                    private Editor editor { get; set; }

                    public HiddenToggleButton(Bindable<bool> hidden)
                    {
                        Action = () => hidden.Value = !hidden.Value;
                        this.hidden = hidden.GetBoundCopy();
                        this.hidden.BindValueChanged(_ => handleVisibilityChange(), true);
                    }

                    private void handleVisibilityChange()
                    {
                        Icon = hidden.Value ? FontAwesome.Solid.EyeSlash : FontAwesome.Solid.Eye;
                        TooltipText = hidden.Value ? "Hidden" : "Visible";
                        editor?.Generate(GenerateKind.Storyboard);
                    }
                }
            }
        }
    }
}
