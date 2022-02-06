// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Cursor;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input.Bindings;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Overlays.Volume;
using osu.Game.Rulesets;
using osu.Game.Screens.Edit;
using sbtw.Editor.Configuration;
using sbtw.Editor.Graphics.UserInterface;
using sbtw.Editor.Overlays;
using sbtw.Editor.Projects;

namespace sbtw.Editor
{
    public abstract partial class Editor : EditorBase, IKeyBindingHandler<GlobalAction>
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private VolumeOverlay volume;
        private EditorSettingsOverlay settings;
        private OutputOverlay output;
        private SetupOverlay setup;
        private NotificationOverlay notifications;
        private LoadingSpinner spinner;
        private EditorPreview preview;
        private Container contentContainer;
        private Container controlContainer;
        private Container middleControlContainer;
        private BindableList<GroupSetting> groups;
        private Bindable<bool> showInterface;
        private Bindable<EditorClock> editorClock;
        private Bindable<EditorBeatmap> editorBeatmap;
        private double lastTrackTime;
        private string lastTrackTitle;
        private bool lastTrackState;

        [BackgroundDependencyLoader]
        private void load()
        {
            SkinManager.CurrentSkinInfo.Value = SkinManager.DefaultLegacySkin.SkinInfo;

            dependencies.CacheAs(this);
            dependencies.CacheAs(setup = new SetupOverlay());
            dependencies.CacheAs(output = new OutputOverlay());
            dependencies.CacheAs(settings = new EditorSettingsOverlay());
            dependencies.CacheAs(notifications = new NotificationOverlay());
            dependencies.CacheAs(editorClock = new NonNullableBindable<EditorClock>(new EditorClock()));
            dependencies.CacheAs(editorBeatmap = new Bindable<EditorBeatmap>());

            Logger.NewEntry += entry =>
            {
                if (entry.Level > LogLevel.Important && entry.Target != null)
                {
                    Schedule(() => notifications.Post(new SimpleErrorNotification
                    {
                        Icon = entry.Level == LogLevel.Important ? FontAwesome.Solid.ExclamationCircle : FontAwesome.Solid.Bomb,
                        Text = entry.Message,
                    }));
                }

                if (entry.Target == null && entry.LoggerName == "script")
                    Schedule(() => output.Print(entry.Message, entry.Level));
            };

            Children = new Drawable[]
            {
                new ProjectWatcher(),
                new VolumeControlReceptor
                {
                    RelativeSizeAxes = Axes.Both,
                    ActionRequested = action => volume.Adjust(action),
                    ScrollActionRequested = (action, amount, isPrecise) => volume.Adjust(action, amount, isPrecise),
                },
                new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new OsuContextMenuContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            contentContainer = new Container
                            {
                                Name = "Content",
                                RelativeSizeAxes = Axes.Both,
                            },
                            controlContainer = new Container
                            {
                                Name = "Control",
                                AlwaysPresent = true,
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    middleControlContainer = new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Padding = new MarginPadding { Horizontal = 10 },
                                        Height = 600,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Children = new Drawable[]
                                        {
                                            new Toolbox
                                            {
                                                Anchor = Anchor.TopRight,
                                                Origin = Anchor.TopRight,
                                            },
                                        }
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Anchor = Anchor.BottomLeft,
                                        Origin = Anchor.BottomLeft,
                                        Margin = new MarginPadding { Bottom = 10 },
                                        Child = new PlaybackControl(),
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 40,
                                        Child = new MainMenuBar(),
                                    },
                                }
                            }
                        }
                    }
                },
                spinner = new LoadingSpinner(true)
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Margin = new MarginPadding { Right = 30, Bottom = 30 },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = 40 },
                    Children = new Drawable[]
                    {
                        setup,
                        notifications.With(d =>
                        {
                            d.Anchor = Anchor.TopRight;
                            d.Origin = Anchor.TopRight;
                        }),
                        output.With(d => d.RelativeSizeAxes = Axes.Both),
                        settings,
                    }
                },
                volume = new VolumeOverlay(),
            };

            showInterface = Session.GetBindable<bool>(EditorSessionStatic.ShowInterface);
            showInterface.BindValueChanged(e => controlContainer.FadeTo(e.NewValue ? 1 : 0, 250, Easing.OutQuint), true);

            Beatmap.ValueChanged += _ => updateControls();
            Project.ValueChanged += _ => updateControls();

            updateControls();
        }

        public void CloseProject()
        {
            preview?.Expire();
            preview = null;
            Beatmap.Disabled = false;
            Beatmap.SetDefault();

            Project.SetDefault();
            Languages.Reset();

            editorClock.SetDefault();
            editorBeatmap.SetDefault();
            Beatmap.Disabled = true;
        }

        public void OpenProject(string path) => OpenProject(Projects.Load(path));

        public void OpenProject(IProject project)
        {
            CloseProject();

            if (project == null)
                return;

            Project.Value = project;

            OpenBeatmap(project.BeatmapSet?.BeatmapSetInfo.Beatmaps.FirstOrDefault());
        }

        public void OpenBeatmap(IBeatmapInfo beatmapInfo)
        {
            if (beatmapInfo == null)
                return;

            Beatmap.Disabled = false;

            lastTrackTime = Beatmap.Value.Track.CurrentTime;
            lastTrackState = Beatmap.Value.Track.IsRunning;
            lastTrackTitle = Beatmap.Value.BeatmapInfo.Metadata.Title;

            Beatmap.Value = Project.Value.BeatmapSet.GetWorkingBeatmap(beatmapInfo);
            Ruleset.Value = beatmapInfo.Ruleset as RulesetInfo;

            Beatmap.Disabled = true;
        }

        public void RefreshBeatmap()
        {
            string current = Beatmap.Value.BeatmapInfo.DifficultyName;
            Project.Value.BeatmapSet.Refresh();
            OpenBeatmap(Project.Value.BeatmapSet?.BeatmapSetInfo.Beatmaps.FirstOrDefault(b => b.DifficultyName == current));
        }

        private CancellationTokenSource reloadTokenSource;

        private void updateControls() => Schedule(() =>
        {
            reloadTokenSource?.Cancel();

            if (Project.Value is DummyProject || Beatmap.Value is DummyWorkingBeatmap)
            {
                preview?.Stop();
                middleControlContainer.Hide();
                contentContainer.Clear();
            }
            else
            {
                reloadTokenSource = new CancellationTokenSource();

                spinner.Show();

                LoadComponentAsync(new EditorPreview(), loaded =>
                {
                    groups = Project.Value.Groups.GetBoundCopy();
                    groups.CollectionChanged += (_, __) => Generate(GenerateKind.Storyboard);

                    contentContainer.Clear();
                    contentContainer.Add(preview = loaded);
                    middleControlContainer.Show();

                    Schedule(() => spinner.Hide());

                    if (Beatmap.Value.BeatmapInfo.Metadata.Title == lastTrackTitle)
                    {
                        preview.Seek(lastTrackTime);

                        if (lastTrackState)
                            preview.Start();
                    }

                    Generate(GenerateKind.Storyboard);
                }, reloadTokenSource.Token);
            }
        });

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            switch (e.Action)
            {
                case GlobalAction.ToggleInGameInterface:
                    showInterface.Value = !showInterface.Value;
                    return true;

                case GlobalAction.ToggleNotifications:
                    notifications.ToggleVisibility();
                    return true;

                case GlobalAction.ToggleSettings:
                    settings.ToggleVisibility();
                    return true;

                case GlobalAction.ToggleChat:
                    output.ToggleVisibility();
                    return true;

                default:
                    return false;
            }
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }

        public override bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
        {
            switch (e.Action)
            {
                case PlatformAction.DocumentNew:
                    setup.Show();
                    return true;

                case PlatformAction.Save:
                    Project.Value?.Save();
                    return true;

                case PlatformAction.DocumentClose:
                    CloseProject();
                    return true;

                default:
                    return base.OnPressed(e);
            }
        }
    }
}
