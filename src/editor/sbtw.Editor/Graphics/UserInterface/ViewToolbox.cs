// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Play.PlayerSettings;
using osuTK;
using sbtw.Editor.Configuration;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

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
            private IBindableList<Script> scripts;

            [BackgroundDependencyLoader]
            private void load(IBindableList<Script> scripts)
            {
                this.scripts = scripts.GetBoundCopy();
                this.scripts.BindCollectionChanged((_, args) => Schedule(() => Children = this.scripts.Select(s => new ScriptListItem(s)).ToList()));
            }
        }

        private class ScriptListItem : CompositeDrawable
        {
            private readonly Script script;

            public ScriptListItem(Script script)
            {
                this.script = script;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                RelativeSizeAxes = Axes.X;
                Height = 40;
                InternalChildren = new Drawable[]
                {
                    new ScriptItemLabel(script.Name)
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                };
            }

            private class ScriptItemLabel : CompositeDrawable
            {
                private readonly string name;

                public ScriptItemLabel(string name)
                {
                    this.name = name;
                }

                [BackgroundDependencyLoader]
                private void load()
                {
                    RelativeSizeAxes = Axes.Both;
                    InternalChildren = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Text = name,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                    };
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

            [BackgroundDependencyLoader]
            private void load(Bindable<IProject> project)
            {
                RelativeSizeAxes = Axes.Both;
                this.project = project.GetBoundCopy();
                this.project.BindValueChanged(e =>
                {
                    Child = new GroupsList
                    {
                        RelativeSizeAxes = Axes.Both,
                        Items = { BindTarget = e.NewValue.Groups }
                    };
                }, true);
            }

            private class GroupsList : OsuRearrangeableListContainer<ElementGroupSetting>
            {
                protected override OsuRearrangeableListItem<ElementGroupSetting> CreateOsuDrawable(ElementGroupSetting item)
                    => new GroupsListItem(item);
            }

            private class GroupsListItem : OsuRearrangeableListItem<ElementGroupSetting>
            {
                public GroupsListItem(ElementGroupSetting item)
                    : base(item)
                {
                }

                protected override Drawable CreateContent() => new GroupsListItemContent(Model);
            }

            private class GroupsListItemContent : Container
            {
                private readonly IconButton target;
                private readonly Bindable<bool> exportToDifficulty;

                public GroupsListItemContent(ElementGroupSetting model)
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
                                target = new IconButton
                                {
                                    Size = new Vector2(40),
                                    Action = () => exportToDifficulty.Value = !exportToDifficulty.Value,
                                }
                            }
                        }
                    };

                    exportToDifficulty = model.ExportToDifficulty.GetBoundCopy();
                    exportToDifficulty.BindValueChanged(_ => handleTargetChange(), true);
                }

                private void handleTargetChange()
                    => target.Icon = exportToDifficulty.Value ? FontAwesome.Solid.FileAlt : FontAwesome.Solid.Globe;
            }
        }
    }
}
