// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Screens.Edit.Components.Menus;
using osuTK;
using osuTK.Graphics;
using sbtw.Editor.Configuration;
using sbtw.Editor.Overlays;
using sbtw.Editor.Platform;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class MainMenuBar : OsuMenu
    {
        private Bindable<WorkingBeatmap> beatmap;
        private Bindable<RulesetInfo> ruleset;
        private Bindable<IProject> project;
        private Bindable<bool> showInterface;

        [Resolved]
        private GameHost host { get; set; }

        [Resolved]
        private EditorBase editorBase { get; set; }

        [Resolved(canBeNull: true)]
        private Editor editor { get; set; }

        [Resolved]
        private EditorSettingsOverlay settings { get; set; }

        [Resolved]
        private OutputOverlay output { get; set; }

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        [Resolved]
        private SetupOverlay setup { get; set; }

        [Resolved]
        private RulesetStore rulesets { get; set; }

        public MainMenuBar()
            : base(Direction.Horizontal, true)
        {
            BackgroundColour = Colour4.FromHex("111");
            RelativeSizeAxes = Axes.Both;

            MaskingContainer.CornerRadius = 0;
            ItemsContainer.Padding = new MarginPadding { Left = 100 };
        }

        [BackgroundDependencyLoader(true)]
        private void load(EditorSessionStatics statics, Bindable<RulesetInfo> ruleset, Bindable<WorkingBeatmap> beatmap, Bindable<IProject> project)
        {
            showInterface = statics.GetBindable<bool>(EditorSessionStatic.ShowInterface);

            this.beatmap = beatmap.GetBoundCopy();
            this.project = project.GetBoundCopy();
            this.ruleset = ruleset.GetBoundCopy();

            this.beatmap.ValueChanged += _ => createItems();
            this.project.ValueChanged += _ => createItems();
            this.ruleset.ValueChanged += _ => createItems();

            createItems();
        }

        private ScheduledDelegate createItemsSchedule;

        private void createItems()
        {
            createItemsSchedule?.Cancel();
            createItemsSchedule = Schedule(() => Items = new[]
            {
                new MenuItem("Project")
                {
                    Items = new[]
                    {
                        new EditorMenuItem("New", MenuItemType.Standard)
                        {
                            Action =
                            {
                                Value = () => setup?.Show(),
                                Disabled = project.Disabled,
                            },
                        },
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Save", MenuItemType.Standard)
                        {
                            Action =
                            {
                                Value = () => (project as IConfigManager)?.Save(),
                                Disabled = project.Value is not IFileBackedProject,
                            },
                        },
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Open", MenuItemType.Standard)
                        {
                            Action =
                            {
                                Value = () => Task.Run(openProject),
                                Disabled = project.Disabled,
                            },
                        },
                        new EditorMenuItem("Open Recent", MenuItemType.Standard)
                        {
                            Action =
                            {
                                Disabled = project.Disabled,
                            },
                        },
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Close")
                        {
                            Action =
                            {
                                Value = project.SetDefault,
                                Disabled = project.Disabled || project.Value is not IFileBackedProject,
                            }
                        },
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Reveal in File Explorer")
                        {
                            Action =
                            {
                                Value = revealProjectInExplorer,
                                Disabled = project.Value is not ICanProvideFiles,
                            }
                        },
                        new EditorMenuItem("Reveal in Code")
                        {
                            Action =
                            {
                                Value = revealProjectInCode,
                                Disabled = project.Value is not ICanProvideFiles,
                            }
                        },
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Export")
                        {
                            Action =
                            {
                                Disabled = project.Value is not IFileBackedProject,
                            },
                        },
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Exit", MenuItemType.Standard) { Action = { Value = host.Exit } },
                    }
                },
                new MenuItem("Beatmap")
                {
                    Items = new[]
                    {
                        new EditorMenuItem("Switch Ruleset")
                        {
                            Items = rulesets.AvailableRulesets
                                .Select(r => new RulesetMenuItem(r, r.ShortName == ruleset.Value.ShortName, r => editorBase.SetRuleset(r))).ToArray()
                        },
                        new EditorMenuItem("Switch Difficulty")
                        {
                            Items = beatmap.Value?.BeatmapInfo.BeatmapSet.Beatmaps?
                                .Select(b => new DifficultyMenuItem(b, b.DifficultyName == beatmap.Value.BeatmapInfo.DifficultyName, b => editorBase.SetBeatmap(b)))
                                .ToArray() ?? new MenuItem[] { new EditorMenuItemSpacer() }
                        },
                        new EditorMenuItem("Reload Beatmap")
                        {
                            Action =
                            {
                                Value = editor.RefreshBeatmap,
                                Disabled = project.Value is not ICanProvideBeatmap,
                            }
                        },
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Reveal Beatmap File")
                        {
                            Action =
                            {
                                Value = revealBeatmapFile,
                                Disabled = project.Value is not ICanProvideBeatmap,
                            }
                        },
                    },
                },
                new MenuItem("Editor")
                {
                    Items = new MenuItem[]
                    {
                        new ToggleMenuItem("Show Interface", MenuItemType.Standard) { State = { BindTarget = showInterface } },
                        new EditorMenuItem("Show Output", MenuItemType.Standard, () => output?.Show()),
                        new EditorMenuItem("Show Notifications", MenuItemType.Standard, () => notifications?.Show()),
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Reload Preview")
                        {
                            Action =
                            {
                                Value = () => editor?.Generate(),
                                Disabled = project.Value is not IFileBackedProject,
                            }
                        },
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
        }

        protected override Menu CreateSubMenu() => new SubMenu();
        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableEditorBarMenuItem(item);

        private async Task openProject()
        {
            if (editorBase is not DesktopEditor desktopEditor)
                return;

            var filters = new[] { new PickerFilter { Description = "sbtw! Projects", Files = new[] { "*.sbtw.json" } } };
            string path = (await desktopEditor.Picker.OpenFileAsync(filters)).FirstOrDefault();

            if (string.IsNullOrEmpty(path))
                return;

            if ((project.Value as IFileBackedProject)?.FullPath == path)
                return;

            project.Value = new JsonBackedProject(path);
        }

        private void revealProjectInExplorer()
        {
            if (project.Value is ICanProvideFiles filesProvider)
                host.OpenFileExternally(filesProvider.Files.GetFullPath("."));
        }

        private void revealProjectInCode()
        {
            if (project.Value is not IFileBackedProject fileBackedProject)
                return;

            if (editorBase is not DesktopEditor desktopEditor)
                return;

            desktopEditor.Studios.Current.Value?.Open(fileBackedProject.Files.GetFullPath("."));
        }

        private void revealBeatmapFile()
        {
            if (project.Value is ICanProvideFiles filesProvider)
                host.OpenFileExternally(filesProvider.BeatmapFiles.GetFullPath($"{beatmap.Value}.osu"));
        }

        private class RulesetMenuItem : StatefulMenuItem<bool>
        {
            public RulesetMenuItem(RulesetInfo rulesetInfo, bool selected, Action<RulesetInfo> action)
                : base(rulesetInfo.ShortName, null)
            {
                State.Value = selected;

                if (!selected)
                    Action.Value = () => action(rulesetInfo);
            }

            public override IconUsage? GetIconForState(bool state)
                => state ? FontAwesome.Solid.Check : null;
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
