// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Graphics.Video;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Screens.Edit.Components.Menus;
using osuTK;
using osuTK.Graphics;
using sbtw.Game.Overlays;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class TopMenuBar : OsuMenu
    {
        [Resolved]
        private GameHost host { get; set; }

        [Resolved(canBeNull: true)]
        private SBTWGame game { get; set; }

        [Resolved(canBeNull: true)]
        private ChatOverlay chat { get; set; }

        [Resolved(canBeNull: true)]
        private NotificationOverlay notifications { get; set; }

        [Resolved]
        private SBTWSettingsOverlay settings { get; set; }

        private Bindable<WorkingBeatmap> beatmap;
        private Bindable<IProject> project;

        public Action RequestNewProject;
        public Action RequestCloseProject;
        public Action<string> RequestOpenProject;
        public Action RequestGenerateStoryboard;
        public Action<BeatmapInfo> RequestDifficultyChange;
        public readonly Bindable<bool> InterfaceVisibility = new Bindable<bool>(true);
        public readonly Bindable<bool> EnableHotReload = new Bindable<bool>();

        public TopMenuBar()
            : base(Direction.Horizontal, true)
        {
            BackgroundColour = Colour4.FromHex("111");
            RelativeSizeAxes = Axes.Both;

            MaskingContainer.CornerRadius = 0;
            ItemsContainer.Padding = new MarginPadding { Left = 100 };

            AddRangeInternal(new Drawable[]
            {
                new StatusBar
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Margin = new MarginPadding { Right = 10 },
                    Padding = new MarginPadding { Horizontal = 5 },
                }
            });
        }

        [BackgroundDependencyLoader(true)]
        private void load(Bindable<WorkingBeatmap> beatmap, Bindable<IProject> project)
        {
            this.beatmap = beatmap?.GetBoundCopy();
            this.project = project?.GetBoundCopy();

            this.beatmap?.BindValueChanged(_ => createItems());
            this.project?.BindValueChanged(_ => createItems());

            createItems();
        }

        private void createItems() => Schedule(() =>
        {
            Items = new[]
            {
                new MenuItem("File")
                {
                    Items = new[]
                    {
                        new EditorMenuItem("New", MenuItemType.Standard, RequestNewProject),
                        new EditorMenuItem("Open", MenuItemType.Standard, openProject),
                        new EditorMenuItem("Save", MenuItemType.Standard, project.Value.Save) { Action = { Disabled = project.Value is DummyProject } },
                        new EditorMenuItem("Close", MenuItemType.Standard, RequestCloseProject) { Action = { Disabled = project.Value is DummyProject } },
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Exit", MenuItemType.Destructive, host.Exit),
                    }
                },
                new ProjectMenuItems(host, project.Value, RequestGenerateStoryboard),
                new BeatmapMenuItems(host, beatmap?.Value, project.Value, RequestDifficultyChange),
                new MenuItem("Editor")
                {
                    Items = new MenuItem[]
                    {
                        new EditorMenuItem("Settings", MenuItemType.Standard, () => settings?.Show()),
                        new ToggleMenuItem("Show Interface") { State = { BindTarget = InterfaceVisibility } },
                        new EditorMenuItemSpacer(),
                        new ToggleMenuItem("Hot Reload") { State = { BindTarget = EnableHotReload } },
                        new ToggleMenuItem(""),
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Show Output", MenuItemType.Standard, () => chat?.Show()),
                        new EditorMenuItem("Show Notifications", MenuItemType.Standard, () => notifications?.Show()),
                    }
                }
            };
        });

        protected override Menu CreateSubMenu() => new SubMenu();

        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableEditorBarMenuItem(item);

        private void openProject()
            => game?.OpenFileDialog(new[] { "*.sbtw.json" }, "sbtw! Projects", RequestOpenProject);

        private class DrawableEditorBarMenuItem : DrawableOsuMenuItem
        {
            private BackgroundBox background;

            public DrawableEditorBarMenuItem(MenuItem item)
                : base(item)
            {
                Anchor = Anchor.CentreLeft;
                Origin = Anchor.CentreLeft;

                StateChanged += stateChanged;
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                ForegroundColour = colours.BlueLight;
                BackgroundColour = Color4.Transparent;
                ForegroundColourHover = Color4.White;
                BackgroundColourHover = colours.Gray3;
            }

            public override void SetFlowDirection(Direction direction)
            {
                AutoSizeAxes = Axes.Both;
            }

            protected override void UpdateBackgroundColour()
            {
                if (State == MenuItemState.Selected)
                    Background.FadeColour(BackgroundColourHover);
                else
                    base.UpdateBackgroundColour();
            }

            protected override void UpdateForegroundColour()
            {
                if (State == MenuItemState.Selected)
                    Foreground.FadeColour(ForegroundColourHover);
                else
                    base.UpdateForegroundColour();
            }

            private void stateChanged(MenuItemState newState)
            {
                if (newState == MenuItemState.Selected)
                    background?.Expand();
                else
                    background?.Contract();
            }

            protected override Drawable CreateBackground() => background = new BackgroundBox();
            protected override DrawableOsuMenuItem.TextContainer CreateTextContainer() => new TextContainer();

            private new class TextContainer : DrawableOsuMenuItem.TextContainer
            {
                public TextContainer()
                {
                    NormalText.Font = NormalText.Font.With(size: 14);
                    BoldText.Font = BoldText.Font.With(size: 14);
                    NormalText.Margin = BoldText.Margin = new MarginPadding { Horizontal = 10, Vertical = MARGIN_VERTICAL };
                }
            }

            private class BackgroundBox : CompositeDrawable
            {
                private readonly Container innerBackground;

                public BackgroundBox()
                {
                    RelativeSizeAxes = Axes.Both;
                    Masking = true;
                    InternalChild = innerBackground = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 4,
                        Child = new Box { RelativeSizeAxes = Axes.Both }
                    };
                }

                /// <summary>
                /// Expands the background such that it doesn't show the bottom corners.
                /// </summary>
                public void Expand() => innerBackground.Height = 2;

                /// <summary>
                /// Contracts the background such that it shows the bottom corners.
                /// </summary>
                public void Contract() => innerBackground.Height = 1;
            }
        }

        private class SubMenu : OsuMenu
        {
            public SubMenu()
                : base(Direction.Vertical)
            {
                OriginPosition = new Vector2(5, 1);
                ItemsContainer.Padding = new MarginPadding { Top = 5, Bottom = 5 };
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                BackgroundColour = colours.Gray3;
            }

            protected override Menu CreateSubMenu() => new SubMenu();

            protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item)
            {
                switch (item)
                {
                    case EditorMenuItemSpacer spacer:
                        return new DrawableSpacer(spacer);
                }

                return base.CreateDrawableMenuItem(item);
            }

            private class DrawableSpacer : DrawableOsuMenuItem
            {
                public DrawableSpacer(MenuItem item)
                    : base(item)
                {
                }

                protected override bool OnHover(HoverEvent e) => true;

                protected override bool OnClick(ClickEvent e) => true;
            }
        }
    }
}
