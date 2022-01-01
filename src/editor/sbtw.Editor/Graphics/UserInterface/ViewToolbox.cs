// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Play.PlayerSettings;
using osuTK;
using sbtw.Editor.Configuration;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class ViewToolbox : EditorTabbedToolbox<string>
    {
        public ViewToolbox()
            : base("View")
        {
            AddTab(@"Layers", new LayersToolboxTab());
            AddTab(@"Groups", new GroupsToolboxTab());
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

            private class GroupsList : OsuRearrangeableListContainer<string>
            {
                protected override OsuRearrangeableListItem<string> CreateOsuDrawable(string item)
                    => new GroupsListItem(item);
            }

            private class GroupsListItem : OsuRearrangeableListItem<string>
            {
                public GroupsListItem(string item)
                    : base(item)
                {
                }

                protected override Drawable CreateContent() => new OsuSpriteText { Text = Model };
            }
        }
    }
}
