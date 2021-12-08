// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Play.PlayerSettings;
using osuTK;
using osuTK.Graphics;

namespace sbtw.Game.Screens.Edit.Toolbox
{
    public class ViewToolboxGroup : PlayerSettingsGroup
    {
        public BindableList<string> Groups => groups.Items;

        public Bindable<bool> PlayfieldVisibility => layers.Playfield;

        private readonly OsuTabControl<ViewTab> tabControl;
        private readonly LayersToolboxTab layers;
        private readonly GroupsToolboxTab groups;

        public ViewToolboxGroup()
            : base("View")
        {
            Children = new Drawable[]
            {
                new Container
                {
                    Height = 24,
                    RelativeSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Height = 1,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Colour = Color4.White.Opacity(0.2f),
                            RelativeSizeAxes = Axes.X,
                        },
                        tabControl = new OsuTabControl<ViewTab>
                        {
                            Items = Enum.GetValues<ViewTab>(),
                            IsSwitchable = true,
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                },
                layers = new LayersToolboxTab(),
                groups = new GroupsToolboxTab(),
            };
        }

        protected override void LoadComplete()
        {
            tabControl.Current.BindValueChanged(e =>
            {
                layers.Alpha = e.NewValue == ViewTab.Layers ? 1 : 0;
                groups.Alpha = e.NewValue == ViewTab.Groups ? 1 : 0;
            }, true);
        }

        protected override bool OnHover(HoverEvent e)
        {
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
        }

        private enum ViewTab
        {
            Layers,
            Groups,
        }

        private abstract class ViewToolboxTab : FillFlowContainer
        {
            public ViewToolboxTab()
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Direction = FillDirection.Vertical;
                Spacing = new Vector2(0, 5);
            }
        }

        private class GroupsToolboxTab : ViewToolboxTab
        {
            public BindableList<string> Items => list.Items;

            private readonly GroupList list;

            public GroupsToolboxTab()
            {
                Child = list = new GroupList();
            }

            private class GroupList : OsuRearrangeableListContainer<string>
            {
                public GroupList()
                {
                    RelativeSizeAxes = Axes.X;
                    AutoSizeAxes = Axes.Y;
                }

                protected override OsuRearrangeableListItem<string> CreateOsuDrawable(string item)
                    => new GroupListItem(item);
            }

            private class GroupListItem : OsuRearrangeableListItem<string>
            {
                public GroupListItem(string item)
                    : base(item)
                {
                }

                protected override Drawable CreateContent() => new OsuSpriteText
                {
                    Text = Model,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Margin = new MarginPadding { Left = 18 },
                };
            }
        }

        private class LayersToolboxTab : ViewToolboxTab
        {
            public readonly Bindable<bool> Playfield = new Bindable<bool>(true);
            private readonly Bindable<bool> video = new Bindable<bool>(true);
            private readonly Bindable<bool> background = new Bindable<bool>(true);
            private readonly Bindable<bool> foreground = new Bindable<bool>(true);
            private readonly Bindable<bool> failing = new Bindable<bool>(true);
            private readonly Bindable<bool> passing = new Bindable<bool>(true);
            private readonly Bindable<bool> overlay = new Bindable<bool>(true);
            private readonly Bindable<bool>[] bindables;

            public LayersToolboxTab()
            {
                Children = new Drawable[]
                {
                    new PlayerCheckbox { LabelText = @"Video", Current = video },
                    new PlayerCheckbox { LabelText = @"Background", Current = background },
                    new PlayerCheckbox { LabelText = @"Foreground", Current = foreground },
                    new PlayerCheckbox { LabelText = @"Failing", Current = failing },
                    new PlayerCheckbox { LabelText = @"Passing", Current = passing },
                    new PlayerCheckbox { LabelText = @"Playfield", Current = Playfield },
                    new PlayerCheckbox { LabelText = @"Overlay", Current = overlay },
                };

                bindables = new[]
                {
                    video,
                    background,
                    foreground,
                    failing,
                    passing,
                    overlay
                };
            }

            [BackgroundDependencyLoader]
            private void load(IBindable<EditorDrawableStoryboard> storyboard)
            {
                storyboard.BindValueChanged(handleStoryboardChange, true);
            }

            private void handleStoryboardChange(ValueChangedEvent<EditorDrawableStoryboard> e)
            {
                foreach (var bindable in bindables)
                {
                    bindable.Disabled = e.NewValue == null;
                    bindable.UnbindEvents();
                }

                if (e.NewValue == null)
                    return;

                bind(e.NewValue, video, "Video");
                bind(e.NewValue, background, "Background");
                bind(e.NewValue, foreground, "Foreground");
                bind(e.NewValue, failing, "Failing");
                bind(e.NewValue, passing, "Passing");
                bind(e.NewValue, overlay, "Overlay");
            }

            private static void bind(EditorDrawableStoryboard storyboard, Bindable<bool> bindable, string layerName)
            {
                bindable.BindValueChanged(e =>
                {
                    var layer = storyboard.Children.FirstOrDefault(l => l.Layer.Name == layerName);

                    if (layer == null)
                        return;

                    layer.Alpha = e.NewValue ? 1 : 0;
                }, true);
            }
        }
    }
}
