// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
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
using osu.Game.Rulesets.Mods;
using osu.Game.Screens.Edit;
using sbtw.Editor.Configuration;
using sbtw.Editor.Generators;
using sbtw.Editor.Graphics.UserInterface;
using sbtw.Editor.Overlays;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

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
        private Container middleControlContainer;
        private Bindable<bool> showInterface;
        private Bindable<EditorClock> editorClock;
        private Bindable<EditorBeatmap> editorBeatmap;
        private double lastTrackTime;
        private string lastTrackTitle;
        private bool lastTrackState;

        [Cached(typeof(IBindableList<Script>))]
        private readonly BindableList<Script> scripts = new BindableList<Script>();

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
            dependencies.CacheAs(editorBeatmap = new NonNullableBindable<EditorBeatmap>(new EditorBeatmap(new Beatmap())));

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
                                            new ViewToolbox
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

            scriptLogger.Add("Welcome to sbtw!");
            scriptLogger.Add("Messages logged by your scripts are shown here.");
            scriptLogger.Add("Error messages that also happen during compilation are also logged here.");

            updateControls();
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

                var generated = await generate(new StoryboardGenerator(Beatmap.Value.BeatmapInfo), GenerateTarget.All, false, generatePreviewTokenSource.Token);

                Schedule(() => preview.SetStoryboard(generated.Result, Project.Value.Resources.Resources));
            }, generatePreviewTokenSource.Token).ContinueWith(handleGeneratorFinish);
        }

        public void GenerateOsb()
        {
            generateOsbTokenSource?.Cancel();

            if (Project.Value is DummyProject || Beatmap.Value is DummyWorkingBeatmap && (preview?.IsLoaded ?? false))
                return;

            generateOsbTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                Schedule(() => spinner.Show());

                var difficulty = await generate(new OsbGenerator(), GenerateTarget.Difficulty, true, generateOsbTokenSource.Token);
                var storyboard = await generate(new OsbGenerator(), GenerateTarget.Storyboard, true, generateOsbTokenSource.Token);

                string file = $"{Beatmap.Value.Metadata.Artist} - {Beatmap.Value.Metadata.Title} ({Beatmap.Value.Metadata.AuthorString})";

                {
                    using var stream = Project.Value.Resources.Storage.GetStream($"{file}.osb", FileAccess.Write);
                    using var writer = new StreamWriter(stream);
                    stream.Position = 0;
                    await writer.WriteAsync(storyboard.ToString());
                }

                {
                    using var stream = Project.Value.Resources.Storage.GetStream($"{file} [{Beatmap.Value.BeatmapInfo.DifficultyName}].osu", FileAccess.ReadWrite, FileMode.Open);
                    using var reader = new StreamReader(stream);
                    using var writer = new StreamWriter(stream);

                    string diff = await reader.ReadToEndAsync();
                    stream.Position = diff.IndexOf("[Events]");
                    await writer.WriteAsync(difficulty.ToString());
                }
            }, generateOsbTokenSource.Token).ContinueWith(handleGeneratorFinish);
        }

        public void CloseProject()
        {
            preview?.Expire();
            preview = null;
            Beatmap.Disabled = false;
            Beatmap.SetDefault();

            if (Project.Value is IDisposable disposable)
                disposable.Dispose();

            Project.SetDefault();

            editorClock.SetDefault();
            editorBeatmap.SetDefault();
            Beatmap.Disabled = true;
        }

        public void OpenProject(string path) => OpenProject(Projects.Load(path));

        public void OpenProject(IProject project)
        {
            CloseProject();
            Project.Value = project;
            OpenBeatmap(Project.Value.BeatmapSet.BeatmapSetInfo.Beatmaps.FirstOrDefault());
        }

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

        private void handleGeneratorFinish(Task task)
        {
            Schedule(() => spinner.Hide());

            if (task.Exception == null)
                return;

            switch (task.Exception.InnerException)
            {
                case UnauthorizedAccessException uae:
                    Logger.Error(uae, "Failed to generate storyboard due to lack of permissions.");
                    break;

                case Exception ex:
                    Logger.Error(ex, "An unknown error has occured while attempting to generate.");
                    break;
            }
        }

        private async Task<GeneratorResult<T, U>> generate<T, U>(Generator<T, U> generator, GenerateTarget target, bool excludeNonVisible, CancellationToken token)
        {
            IEnumerable<ElementGroupSetting> groups = Project.Value.Groups;

            if (target == GenerateTarget.Difficulty)
                groups = groups.Where(g => g.ExportToDifficulty.Value);

            if (target == GenerateTarget.Storyboard)
                groups = groups.Where(g => !g.ExportToDifficulty.Value);

            if (excludeNonVisible)
                groups = groups.Where(g => g.Visible.Value);

            var compiled = await Languages.CompileAsync(Project.Value.Files, null, token);

            var generated = await generator.GenerateAsync(new GeneratorConfig
            {
                Scripts = compiled,
                Storage = Project.Value.Files,
                Beatmap = Beatmap.Value.GetPlayableBeatmap(Beatmap.Value.BeatmapInfo.Ruleset, new List<Mod>(), token),
                Waveform = Beatmap.Value.Waveform,
                Ordering = groups.Select(g => g.Name),
            }, token);

            if (generated.Assets.Any())
                Project.Value.Assets.Generate(generated.Assets);

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

            Schedule(() =>
            {
                var added = generated.Groups.Except(Project.Value.Groups.Select(g => g.Name));
                var removed = Project.Value.Groups.Select(g => g.Name).Except(generated.Groups);

                Project.Value.Groups.AddRange(added.Select(g => new ElementGroupSetting { Name = g }));
                Project.Value.Groups.RemoveAll(g => removed.Contains(g.Name));

                scripts.Clear();
                scripts.AddRange(compiled);
            });

            return generated;
        }

        private enum GenerateTarget
        {
            All,
            Difficulty,
            Storyboard,
        }

        private void updateControls() => Schedule(() =>
        {
            reloadTokenSource?.Cancel();
            generatePreviewTokenSource?.Cancel();

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
                middleControlContainer.Show();

                LoadComponentAsync(new EditorPreview(), loaded =>
                {
                    contentContainer.Clear();
                    contentContainer.Add(preview = loaded);

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
