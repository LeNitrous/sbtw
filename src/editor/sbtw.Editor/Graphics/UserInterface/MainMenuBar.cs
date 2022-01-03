// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Screens.Edit.Components.Menus;
using osuTK;
using osuTK.Graphics;
using sbtw.Editor.Configuration;
using sbtw.Editor.Overlays;
using sbtw.Editor.Projects;
using sbtw.Editor.Studios;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class MainMenuBar : OsuMenu
    {
        private Bindable<WorkingBeatmap> beatmap;
        private Bindable<IProject> project;
        private Bindable<Studio> studio;
        private Bindable<bool> showInterface;

        [Resolved]
        private GameHost host { get; set; }

        [Resolved]
        private Bindable<IProject> resolvedProject { get; set; }

        [Resolved]
        private Bindable<WorkingBeatmap> resolvedBeatmap { get; set; }

        [Resolved]
        private Bindable<Studio> resolvedStudio { get; set; }

        [Resolved]
        private EditorSessionStatics statics { get; set; }

        [Resolved(canBeNull: true)]
        private Editor editor { get; set; }

        [Resolved(canBeNull: true)]
        private EditorSettingsOverlay settings { get; set; }

        [Resolved(canBeNull: true)]
        private OutputOverlay output { get; set; }

        [Resolved(canBeNull: true)]
        private NotificationOverlay notifications { get; set; }

        [Resolved(canBeNull: true)]
        private SetupOverlay setup { get; set; }

        public MainMenuBar()
            : base(Direction.Horizontal, true)
        {
            BackgroundColour = Colour4.FromHex("111");
            RelativeSizeAxes = Axes.Both;

            MaskingContainer.CornerRadius = 0;
            ItemsContainer.Padding = new MarginPadding { Left = 100 };
        }

        [BackgroundDependencyLoader(true)]
        private void load()
        {
            beatmap = resolvedBeatmap.GetBoundCopy();
            project = resolvedProject.GetBoundCopy();
            studio = resolvedStudio.GetBoundCopy();
            showInterface = statics.GetBindable<bool>(EditorSessionStatic.ShowInterface);

            beatmap.ValueChanged += _ => createItems();
            project.ValueChanged += _ => createItems();

            createItems();
        }

        private void createItems() => Schedule(() => Items = new[]
        {
            new MenuItem("Project")
            {
                Items = new[]
                {
                    new EditorMenuItem("New", MenuItemType.Standard, () => setup?.Show()),
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("Save", MenuItemType.Standard)
                    {
                        Action =
                        {
                            Value = () => project.Value.Save(),
                            Disabled = project.Value is DummyProject,
                        }
                    },
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("Open", MenuItemType.Standard, openProject),
                    new EditorMenuItem("Open Recent", MenuItemType.Standard),
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("Close")
                    {
                        Action =
                        {
                            Value = () => editor?.CloseProject(),
                            Disabled = project.Value is DummyProject,
                        }
                    },
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("Reveal in File Explorer")
                    {
                        Action =
                        {
                            Value = revealProject,
                            Disabled = project.Value is DummyProject
                        }
                    },
                    new EditorMenuItem("Reveal in Code")
                    {
                        Action =
                        {
                            Value = revealProjectWorkspace,
                            Disabled = project.Value is DummyProject
                        }
                    },
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("Exit", MenuItemType.Standard) { Action = { Value = host.Exit } },
                }
            },
            new MenuItem("Beatmap")
            {
                Items = new[]
                {
                    new EditorMenuItem("Switch Difficulty")
                    {
                        Items = beatmap.Value?.BeatmapInfo.BeatmapSet.Beatmaps?
                            .Select(b => new DifficultyMenuItem(b, b.DifficultyName == beatmap.Value.BeatmapInfo.DifficultyName, b => editor?.OpenBeatmap(b)))
                            .ToArray() ?? new MenuItem[] { new EditorMenuItemSpacer() }
                    },
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("Reload Beatmap")
                    {
                        Action =
                        {
                            Value = () => editor?.RefreshBeatmap(),
                            Disabled = beatmap.Value is DummyWorkingBeatmap
                        }
                    },
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("Reveal Beatmap File")
                    {
                        Action =
                        {
                            Value = revealBeatmapFile,
                            Disabled = beatmap.Value is DummyWorkingBeatmap
                        }
                    },
                },
            },
            new MenuItem("Storyboard")
            {
                Items = new[]
                {
                    new EditorMenuItem("Generate Preview")
                    {
                        Action =
                        {
                            Value = () => editor?.GeneratePreview(),
                            Disabled = project.Value is DummyProject
                        }
                    },
                    new EditorMenuItem("Generate .osb")
                    {
                        Action =
                        {
                            Value = () => editor?.GenerateOsb(),
                            Disabled = project.Value is DummyProject
                        }
                    },
                }
            },
            new MenuItem("Editor")
            {
                Items = new MenuItem[]
                {
                    new ToggleMenuItem("Show Interface", MenuItemType.Standard) { State = { BindTarget = showInterface } },
                    new EditorMenuItem("Show Output", MenuItemType.Standard, () => output?.Show()),
                    new EditorMenuItem("Show Notifications", MenuItemType.Standard, () => notifications?.Show()),
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("Open Settings", MenuItemType.Standard, () => settings?.Show()),
                },
            },
            new MenuItem("Help")
            {
                Items = new[]
                {
                    new EditorMenuItem("Getting Started"),
                    new EditorMenuItem("Tips and Tricks"),
                    new EditorMenuItem("Documentation"),
                    new EditorMenuItemSpacer(),
                    new EditorMenuItem("sbtw! on GitHub") { Action = { Value = () => host.OpenUrlExternally("https://github.com/lenitrous/sbtw") } }
                }
            }
        });

        protected override Menu CreateSubMenu() => new SubMenu();
        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableEditorBarMenuItem(item);

        private void openProject()
        {
            if (editor == null)
                return;

            Schedule(async () =>
            {
                string path = await editor.RequestSingleFileAsync(extensions: new[] { ".json" });

                if (string.IsNullOrEmpty(path))
                    return;

                editor.OpenProject(path);
            });
        }

        private void revealProject() => host.OpenFileExternally(project.Value.Path);
        private void revealProjectWorkspace() => studio.Value?.Open(project.Value.Path);

        private void revealBeatmapFile()
        {
            string path = Directory.GetFiles(Path.Combine(project.Value.Path, "Beatmap"))
                .FirstOrDefault(f => f.Contains($"[{beatmap.Value.BeatmapInfo.DifficultyName}]"));

            if (string.IsNullOrEmpty(path))
                return;

            host.OpenFileExternally(path);
        }

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

                public void Expand() => innerBackground.Height = 2;
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
