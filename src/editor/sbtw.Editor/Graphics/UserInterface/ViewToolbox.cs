// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Play.PlayerSettings;
using osuTK;
using sbtw.Editor.Configuration;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Studios;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class ViewToolbox : EditorTabbedToolbox<string>
    {
        public ViewToolbox()
            : base("Tools")
        {
            AddTab(@"Layers", new LayersToolboxTab());
            AddTab(@"Groups", new GroupsToolboxTab());
            AddTab(@"Scirpts", new ScriptsToolboxTab());
        }

        protected override TabControl<string> CreateTabControl() => new ViewToolboxTabControl { RelativeSizeAxes = Axes.Both };

        private class ViewToolboxTabControl : OsuTabControl<string>
        {
            protected override TabItem<string> CreateTabItem(string value) => new ToolBoxTabItem(value);

            private class ToolBoxTabItem : OsuTabItem
            {
                public ToolBoxTabItem(string value)
                    : base(value)
                {
                }

                protected override bool OnHover(HoverEvent e)
                {
                    if (Active.Value)
                        FadeHovered();

                    return false;
                }
            }
        }

        private class ScriptsToolboxTab : FillFlowContainer
        {
            private readonly BindableList<ScriptGenerationResult> scripts = new BindableList<ScriptGenerationResult>();
            private Bindable<IProject> project;

            protected override Container<Drawable> Content { get; }

            public ScriptsToolboxTab()
            {
                RelativeSizeAxes = Axes.Both;
                InternalChild = new OsuScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = Content = new FillFlowContainer
                    {
                        Direction = FillDirection.Vertical,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(Bindable<IProject> project)
            {
                this.project = project.GetBoundCopy();
                this.project.BindValueChanged(e =>
                {
                    scripts.UnbindBindings();

                    if (e.NewValue is not DummyProject)
                        scripts.BindTo(e.NewValue.Scripts);
                }, true);

                scripts.BindCollectionChanged((_, args) => Schedule(() => Children = scripts.Select(s => new ScriptListItem(s)).ToList()), true);
            }
        }

        private class ScriptListItem : CompositeDrawable
        {
            private readonly ScriptGenerationResult script;

            public ScriptListItem(ScriptGenerationResult script)
            {
                this.script = script;
            }

            [BackgroundDependencyLoader]
            private void load(Bindable<Studio> studio)
            {
                RelativeSizeAxes = Axes.X;
                Height = 40;
                InternalChildren = new Drawable[]
                {
                    new LabelSpriteText(script)
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding { Left = 30 },
                    },
                    new BugIndicator
                    {
                        Icon = FontAwesome.Solid.Bug,
                        Size = new Vector2(14),
                        Alpha = script.Exception != null ? 1 : 0,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                    new IconButton
                    {
                        Icon = FontAwesome.Regular.Edit,
                        Size = new Vector2(40),
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Action = () => studio.Value?.Open(script.Path),
                        TooltipText = @"Reveal script in code.",
                    }
                };
            }

            private class BugIndicator : SpriteIcon, IHasTooltip
            {
                public LocalisableString TooltipText => @"There were errors when running this script.";
            }

            private class LabelSpriteText : OsuSpriteText, IHasTooltip
            {
                public LocalisableString TooltipText { get; }

                public LabelSpriteText(ScriptGenerationResult script)
                {
                    Text = script.Name;
                    TooltipText = script.Path;
                }
            }
        }

        private class LayersToolboxTab : FillFlowContainer
        {
            [BackgroundDependencyLoader]
            private void load(EditorSessionStatics statics)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Direction = FillDirection.Vertical;
                Spacing = new Vector2(0, 5);
                Children = new Drawable[]
                {
                    new PlayerCheckbox { LabelText = @"Video", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowVideo) },
                    new PlayerCheckbox { LabelText = @"Background", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowBackground) },
                    new PlayerCheckbox { LabelText = @"Failing", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowFailing)},
                    new PlayerCheckbox { LabelText = @"Passing", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowPassing) },
                    new PlayerCheckbox { LabelText = @"Foreground", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowForeground) },
                    new PlayerCheckbox { LabelText = @"Playfield", Current = statics.GetBindable<bool>(EditorSessionStatic.ShowPlayfield) },
                    new PlayerCheckbox { LabelText = @"Overlay", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowOverlay) },
                };
            }
        }

        private class GroupsToolboxTab : Container
        {
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
                    list.Items.UnbindBindings();

                    if (e.NewValue is not DummyProject)
                        list.Items.BindTo(e.NewValue.Groups);
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
                                Icon = FontAwesome.Solid.Circle;
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
