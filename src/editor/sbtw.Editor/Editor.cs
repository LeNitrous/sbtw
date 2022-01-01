// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
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
using sbtw.Editor.Configuration;
using sbtw.Editor.Generators;
using sbtw.Editor.Graphics.UserInterface;
using sbtw.Editor.Overlays;
using sbtw.Editor.Projects;

namespace sbtw.Editor
{
    public abstract class Editor : EditorBase, IKeyBindingHandler<GlobalAction>
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private readonly Logger scriptLogger = Logger.GetLogger("script");
        private VolumeOverlay volume;
        private EditorSettingsOverlay settings;
        private OutputOverlay output;
        private SetupOverlay setup;
        private NotificationOverlay notifications;
        private LoadingSpinner spinner;
        private EditorPreview preview;
        private Container contentContainer;
        private Container controlContainer;
        private Container topControlContainer;
        private Container bottomControlContainer;
        private Bindable<bool> showInterface;
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
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding { Top = 50, Bottom = 90, Horizontal = 10 },
                                        Children = new Drawable[]
                                        {
                                            new VariablesToolbox(),
                                            new ViewToolbox
                                            {
                                                Anchor = Anchor.TopRight,
                                                Origin = Anchor.TopRight,
                                            },
                                        }
                                    },
                                    bottomControlContainer = new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Margin = new MarginPadding { Bottom = 10 },
                                    },
                                    topControlContainer = new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 40,
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
            showInterface.BindValueChanged(e => controlContainer.Alpha = e.NewValue ? 1 : 0, true);

            Beatmap.ValueChanged += _ => reloadControls();
            Project.ValueChanged += _ => reloadControls();

            scriptLogger.Add("Welcome to sbtw!");
            scriptLogger.Add("Messages logged by your scripts are shown here.");
            scriptLogger.Add("Error messages that also happen during compilation are also logged here.");

            reloadControls();
        }

        private CancellationTokenSource generatePreviewTokenSource;
        private CancellationTokenSource generateOsbTokenSource;
        private CancellationTokenSource reloadTokenSource;

        public void GeneratePreview()
        {
            generatePreviewTokenSource?.Cancel();

            if (Project.Value is DummyProject || Beatmap.Value is DummyWorkingBeatmap && (preview?.IsLoaded ?? false))
                return;

            generatePreviewTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                Schedule(() => spinner.Show());

                var generated = await generate(new StoryboardGenerator(Beatmap.Value.BeatmapInfo), generatePreviewTokenSource.Token);

                Schedule(() =>
                {
                    Project.Value.Groups.Clear();
                    Project.Value.Groups.AddRange(generated.Groups);
                    Project.Value.Variables.Clear();
                    Project.Value.Variables.AddRange(generated.Variables.ToList());
                    preview.SetStoryboard(generated.Result, Project.Value.Resources.Resources);
                    spinner.Hide();
                });
            }, generatePreviewTokenSource.Token).ContinueWith(handleGeneratorExceptions, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void GenerateOsb()
        {
            generateOsbTokenSource?.Cancel();

            if (Project.Value is DummyProject || Beatmap.Value is DummyWorkingBeatmap && (preview?.IsLoaded ?? false))
                return;

            generateOsbTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                var path = await RequestSaveFileAsync(suggestedName: "generated.osb", extensions: new[] { "osb" });

                if (string.IsNullOrEmpty(path))
                    return;

                Schedule(() => spinner.Show());

                var generated = await generate(new OsbGenerator(), generateOsbTokenSource.Token);

                await File.WriteAllTextAsync(path, generated.Result.ToString());

                Schedule(() => spinner.Hide());
            }, generateOsbTokenSource.Token).ContinueWith(handleGeneratorExceptions, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void CloseProject()
        {
            Beatmap.Disabled = false;
            Project.SetDefault();
            Beatmap.SetDefault();
            Beatmap.Disabled = true;
        }

        public void OpenProject(string path)
        {
            Project.Value = Projects.Load(path);
            OpenBeatmap(Project.Value.BeatmapSet.BeatmapSetInfo.Beatmaps.FirstOrDefault());
        }

        public void OpenProject(IProject project) => OpenProject(project.Path);

        public void OpenBeatmap(IBeatmapInfo beatmapInfo)
        {
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
            OpenBeatmap(Project.Value.BeatmapSet.BeatmapSetInfo.Beatmaps.FirstOrDefault(b => b.DifficultyName == current));
        }

        private void handleGeneratorExceptions(Task task)
        {
            switch (task.Exception.InnerException)
            {
                case UnauthorizedAccessException uae:
                    Logger.Error(uae, "Failed to generate storyboard due to lack of permissions.");
                    break;
            }

            Schedule(() => spinner.Hide());
        }

        private async Task<GeneratorResult<T, U>> generate<T, U>(Generator<T, U> generator, CancellationToken token)
        {
            var generated = await generator.GenerateAsync(new GeneratorConfig
            {
                Scripts = await Languages.CompileAsync(Project.Value.Files, token),
                Ordering = Project.Value.Groups,
                Variables = Project.Value.Variables,
            }, token);

            if (generated.Faulted.Any())
            {
                var notification = new SimpleErrorNotification
                {
                    Icon = FontAwesome.Solid.Bomb,
                    Text = @"There are scripts that failed to run. See output for more details.",

                };

                notification.Closed += () => output.Show();
                notifications.Post(notification);
            }

            return generated;
        }

        private void reloadControls()
        {
            reloadTokenSource?.Cancel();
            generatePreviewTokenSource?.Cancel();

            preview?.Stop();
            preview = null;

            topControlContainer.Clear();
            contentContainer.Clear();
            bottomControlContainer.Clear();

            reloadTokenSource = new CancellationTokenSource();

            LoadComponentAsync(new MainMenuBar(), loaded => topControlContainer.Child = loaded, reloadTokenSource.Token);

            if (Project.Value is DummyProject || Beatmap.Value is DummyWorkingBeatmap)
                return;

            spinner.Show();

            LoadComponentAsync(new EditorPreview(), loaded =>
            {
                contentContainer.Add(preview = loaded);
                bottomControlContainer.Add(loaded.Controls.CreateProxy());
                Schedule(() => spinner.Hide());

                if (Beatmap.Value.BeatmapInfo.Metadata.Title == lastTrackTitle)
                {
                    preview.Seek(lastTrackTime);

                    if (lastTrackState)
                        preview.Start();
                }

                GeneratePreview();
            }, reloadTokenSource.Token);
        }

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
