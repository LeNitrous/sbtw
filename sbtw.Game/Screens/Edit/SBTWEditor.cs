// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input.Bindings;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens;
using osu.Game.Screens.Backgrounds;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Components;
using osu.Game.Screens.Edit.Components.Menus;
using osu.Game.Screens.Edit.Components.Timelines.Summary;
using osuTK;
using osuTK.Input;
using sbtw.Game.Projects;
using sbtw.Game.Screens.Setup;
using sbtw.Game.Scripting;

namespace sbtw.Game.Screens.Edit
{
    [Cached(typeof(IBeatSnapProvider))]
    public class SBTWEditor : OsuScreen, IBeatSnapProvider, IKeyBindingHandler<GlobalAction>
    {
        public override float BackgroundParallaxAmount => 0.0f;

        public override bool? AllowTrackAdjustments => false;

        public override bool AllowBackButton => false;

        public override bool DisallowExternalBeatmapRulesetChanges => true;

        protected override BackgroundScreen CreateBackground() => new BackgroundScreenBlack();

        private Container interfaceContainer;
        private Container previewContainer;
        private Container playfieldContainer;
        private IBeatmap playableBeatmap;
        private EditorClock clock;
        private EditorBeatmap editorBeatmap;
        private DependencyContainer dependencies;

        private MenuItem[] projectMenuItems;
        private MenuItem[] beatmapMenuItems;

        private readonly Bindable<bool> interfaceVisibility = new Bindable<bool>(true);
        private readonly Bindable<bool> playfieldVisibility = new Bindable<bool>(true);
        private readonly BindableList<string> groups = new BindableList<string>();
        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();
        private readonly Bindable<EditorDrawableStoryboard> storyboard = new Bindable<EditorDrawableStoryboard>();

        [Resolved]
        private Bindable<Project> project { get; set; }

        [Resolved]
        private GameHost host { get; set; }

        [Resolved]
        private SBTWGame game { get; set; }

        [Resolved]
        private SBTWLoader loader { get; set; }

        [Resolved]
        private ChatOverlay chatOverlay { get; set; }

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        public int BeatDivisor => beatDivisor.Value;

        [BackgroundDependencyLoader]
        private void load(OsuColour colours, NetProcessListener listener)
        {
            var loadableBeatmap = Beatmap.Value;

            if (loadableBeatmap is DummyWorkingBeatmap)
            {
                loadableBeatmap.LoadTrack();
            }

            try
            {
                playableBeatmap = loadableBeatmap.GetPlayableBeatmap(loadableBeatmap.BeatmapInfo.Ruleset);
            }
            catch (Exception)
            {
                notifications.Post(new SimpleErrorNotification
                {
                    Text = @"Failed to load beatmap!",
                    Icon = FontAwesome.Solid.Bomb,
                });
                return;
            }

            dependencies.CacheAs<IBindable<EditorDrawableStoryboard>>(storyboard);

            beatDivisor.Value = playableBeatmap.BeatmapInfo.BeatDivisor;
            dependencies.CacheAs(beatDivisor);

            clock = new EditorClock(playableBeatmap, beatDivisor) { IsCoupled = false };
            clock.ChangeSource(loadableBeatmap.Track);

            dependencies.CacheAs(clock);
            AddInternal(clock);

            AddInternal(editorBeatmap = new EditorBeatmap(playableBeatmap, loadableBeatmap.Skin));
            dependencies.CacheAs(editorBeatmap);

            var controlsTop = new Container
            {
                Name = "menu",
                RelativeSizeAxes = Axes.X,
                Height = 40,
                Child = new SBTWEditorMenuBar
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Both,
                    Items = new[]
                    {
                        new MenuItem("File")
                        {
                            Items = new[]
                            {
                                new EditorMenuItem("New", MenuItemType.Standard, () => this.Push(new SetupScreen())),
                                new EditorMenuItem("Open", MenuItemType.Standard, openProjectFile),
                                new EditorMenuItem("Close", MenuItemType.Standard, () => loader.CloseProject()),
                                new EditorMenuItemSpacer(),
                                new EditorMenuItem("Exit", MenuItemType.Destructive, () => host.Exit()),
                            }
                        },
                        new MenuItem("Project")
                        {
                            Items = projectMenuItems = new[]
                            {
                                new EditorMenuItem("Run", MenuItemType.Highlighted, generateStoryboard),
                                new EditorMenuItemSpacer(),
                                new EditorMenuItem("Build", MenuItemType.Standard, () => project.Value?.Build()),
                                new EditorMenuItem("Clean", MenuItemType.Standard, () => project.Value?.Clean()),
                                new EditorMenuItem("Restore", MenuItemType.Standard, () => project.Value?.Restore()),
                                new EditorMenuItemSpacer(),
                                new EditorMenuItem("Reveal in File Explorer", MenuItemType.Standard, presentProjectFolder) { Action = { Disabled = project.Value == null } },
                                new EditorMenuItem("Open in Code", MenuItemType.Standard, presentProjectFolderInCode) { Action = { Disabled = !ProjectHelper.EDITORS.Any() || project.Value == null } }
                            },
                        },
                        new MenuItem("Beatmap")
                        {
                            Items = beatmapMenuItems = new[]
                            {
                                new EditorMenuItem("Select Difficulty")
                                {
                                    Items = playableBeatmap.BeatmapInfo.BeatmapSet.Beatmaps?
                                        .Select(b => new DifficultyMenuItem(b, b.Version == Beatmap.Value.BeatmapInfo.Version, (b) => loader.ChangeBeatmap(b.Version)))
                                        .ToArray() ?? new MenuItem[] { new EditorMenuItemSpacer() },
                                },
                                new EditorMenuItemSpacer(),
                                new EditorMenuItem("Reload Beatmap", MenuItemType.Standard, () => loader.ReloadBeatmap()),
                                new EditorMenuItemSpacer(),
                                new EditorMenuItem("Reveal Beatmap Folder", MenuItemType.Standard, presentBeatmapFolder),
                                new EditorMenuItem("Open Difficulty File", MenuItemType.Standard, presentBeatmapDifficultyFile),
                                new EditorMenuItem("Open Storyboard File", MenuItemType.Standard, presentBeatmapStoryboardFile),
                            }
                        },
                        new MenuItem("View")
                        {
                            Items = new MenuItem[]
                            {
                                new ToggleMenuItem("Show Interface") { State = { BindTarget = interfaceVisibility } },
                                new EditorMenuItemSpacer(),
                                new EditorMenuItem("Show Logs", MenuItemType.Standard, () => chatOverlay.Show()),
                                new EditorMenuItem("Show Notifications", MenuItemType.Standard, () => notifications?.Show()),
                            }
                        }
                    }
                }
            };

            var controlsBottom = new Container
            {
                Name = "progress",
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                RelativeSizeAxes = Axes.X,
                Height = 60,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = colours.Gray2
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Vertical = 5, Horizontal = 10 },
                        Child = new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ColumnDimensions = new[]
                            {
                                new Dimension(GridSizeMode.Absolute, 220),
                                new Dimension(),
                                new Dimension(GridSizeMode.Absolute, 220)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding { Right = 10 },
                                        Child = new TimeInfoContainer { RelativeSizeAxes = Axes.Both },
                                    },
                                    new SummaryTimeline
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding { Left = 10 },
                                        Child = new PlaybackControl { RelativeSizeAxes = Axes.Both },
                                    }
                                },
                            }
                        },
                    }
                }
            };

            var controlsLeft = new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.Y,
                Spacing = new Vector2(5),
                Width = 250,
                Children = new Drawable[]
                {
                    new ToolboxGroup("Scripts"),
                    new ToolboxGroup("Properties"),
                },
            };

            var controlsRight = new FillFlowContainer
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.Y,
                Spacing = new Vector2(5),
                Width = 250,
                Children = new Drawable[]
                {
                    new GroupsToolboxGroup { Items = { BindTarget = groups } },
                    new LayersToolboxGroup { Playfield = { BindTarget = playfieldVisibility } },
                },
            };

            AddRangeInternal(new Drawable[]
            {
                previewContainer = new Container
                {
                    Name = "preview",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Children = new Drawable[]
                    {
                        new BeatmapBackground(Beatmap.Value),
                    },
                },
                playfieldContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                },
                interfaceContainer = new Container
                {
                    Name = "interface",
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding(5),
                            Margin = new MarginPadding { Top = 40 },
                            Children = new Drawable[]
                            {
                                controlsLeft,
                                controlsRight,
                            },
                        },
                        controlsTop,
                        controlsBottom,
                    },
                }
            });

            foreach (var item in projectMenuItems.Take(5))
            {
                if (item is EditorMenuItem)
                    item.Action.Disabled = !ProjectHelper.HAS_DOTNET || project.Value == null;
            }

            foreach (var item in beatmapMenuItems.Skip(1))
            {
                if (item is EditorMenuItem)
                    item.Action.Disabled = project.Value == null;
            }

            listener.State.BindValueChanged(e =>
            {
                if (project.Value == null)
                    return;

                Schedule(() =>
                {
                    foreach (var item in projectMenuItems.Take(projectMenuItems.Length - 2))
                        item.Action.Disabled = e.NewValue != NetProcessStatus.Exited;
                });
            }, true);

            interfaceVisibility.ValueChanged += e => interfaceContainer.FadeTo(e.NewValue ? 1 : 0, 250, Easing.OutQuint);
            playfieldVisibility.ValueChanged += e => playfieldContainer.FadeTo(e.NewValue ? 1 : 0, 250, Easing.OutQuint);

            if (Ruleset.Value == null || loadableBeatmap is DummyWorkingBeatmap)
                return;

            var ruleset = Ruleset.Value.CreateInstance();
            LoadComponentAsync(new EditorSkinProvidingContainer(editorBeatmap)
            {
                Child = new EditorDrawableRuleset(playableBeatmap, ruleset.CreateDrawableRulesetWith(playableBeatmap), ruleset.GetAutoplayMod())
                {
                    Clock = clock,
                    ProcessCustomClock = false,
                },
            }, playfieldContainer.Add);
        }

        protected override void Update()
        {
            base.Update();
            clock?.ProcessFrame();
        }

        public override bool OnExiting(IScreen next)
        {
            Beatmap.Value.Track.Stop();
            return base.OnExiting(next);
        }

        private bool playOnResume;

        public override void OnSuspending(IScreen next)
        {
            playOnResume = Beatmap.Value.Track.IsRunning;
            Beatmap.Value.Track.Stop();
            base.OnSuspending(next);
        }

        public override void OnResuming(IScreen last)
        {
            if (playOnResume)
                Beatmap.Value.Track.Start();

            base.OnResuming(last);
        }

        public double SnapTime(double time, double? referenceTime = null) => editorBeatmap?.SnapTime(time, referenceTime) ?? 0.0;

        public double GetBeatLengthAtTime(double referenceTime) => editorBeatmap?.GetBeatLengthAtTime(referenceTime) ?? 0.0;

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            switch (e.Action)
            {
                case GlobalAction.ToggleInGameInterface:
                    interfaceVisibility.Value = !interfaceVisibility.Value;
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    seek(e, -1);
                    return true;

                case Key.Right:
                    seek(e, 1);
                    return true;
            }

            return base.OnKeyDown(e);
        }

        private void seek(UIEvent e, int direction)
        {
            double amt = e.ShiftPressed ? 4 : 1;
            bool isPlaying = clock.IsRunning;

            if (isPlaying)
                amt *= beatDivisor.Value;

            if (direction < 1)
                clock.SeekBackward(!isPlaying, amt);
            else
                clock.SeekForward(!isPlaying, amt);
        }

        private void generateStoryboard()
        {
            storyboard.Value?.Expire();

            using var generator = new StoryboardGenerator(project.Value);
            var generated = generator.Generate(groups, out var added, out var removed);
            storyboard.Value = new EditorDrawableStoryboard(generated) { Clock = clock };

            foreach (var item in added)
                groups.Add(item);

            foreach (var item in removed)
                groups.Remove(item);

            LoadComponentAsync(storyboard.Value, previewContainer.Add);
        }

        private void presentBeatmapDifficultyFile()
        {
            string path = Directory.GetFiles(project.Value.BeatmapPath)
                .FirstOrDefault(f => f.Contains($"[{playableBeatmap.BeatmapInfo.Version}]"));

            if (!string.IsNullOrEmpty(path))
                host.OpenFileExternally(path);
        }

        private void presentBeatmapFolder()
            => host.OpenFileExternally(project.Value.BeatmapPath);

        private void presentBeatmapStoryboardFile()
        {
            string path = Directory.GetFiles(project.Value.BeatmapPath)
                .FirstOrDefault(f => Path.GetExtension(f) == ".osb");

            if (!string.IsNullOrEmpty(path))
                host.OpenFileExternally(path);
        }

        private void presentProjectFolder()
            => host.OpenFileExternally(Path.GetDirectoryName(project.Value.Path));

        private void presentProjectFolderInCode()
        {
            if (!ProjectHelper.EDITORS.Any())
                return;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ProjectHelper.EDITORS.FirstOrDefault().Key,
                    Arguments = project.Value.Path,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                }
            };

            process.Start();
        }

        private void openProjectFile()
            => game.OpenFileDialog(new[] { "*.csproj" }, "MSBuild Projects", path => Schedule(() => loader.LoadProject(path)));
    }
}
