// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Threading;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input.Bindings;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Overlays.Volume;
using osuTK;
using osuTK.Input;
using sbtw.Editor.Configuration;
using sbtw.Editor.Generators;
using sbtw.Editor.Graphics.UserInterface;
using sbtw.Editor.Graphics.UserInterface.Bottom;
using sbtw.Editor.Graphics.UserInterface.Toolbox;
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

        private VolumeOverlay volume;
        private EditorSettingsOverlay settings;
        private OutputOverlay output;
        private SetupOverlay setup;
        private NotificationOverlay notifications;
        private LoadingSpinner spinner;
        private EditorPreview preview;
        private Container contentContainer;
        private Container toolboxContainer;
        private Container controlContainer;
        private Container bottomContainer;
        private FileWatcher watcher;
        private Bindable<bool> showInterface;

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

                if (entry.Target == null && entry.LoggerName == ScriptLogger.Name)
                {
                    Schedule(() => output.AddLine(entry.Message, entry.Level));

                    if (entry.Level != LogLevel.Error)
                        return;

                    Schedule(() => notifications.Post(new SimpleErrorNotification
                    {
                        Icon = FontAwesome.Solid.ExclamationTriangle,
                        Text = @"Errors have occured during script execution. See output for more details.",
                        Activated = () =>
                        {
                            output.Show();
                            return true;
                        }
                    }));
                }
            };

            Children = new Drawable[]
            {
                new VolumeControlReceptor
                {
                    RelativeSizeAxes = Axes.Both,
                    ActionRequested = action => volume.Adjust(action),
                    ScrollActionRequested = (action, amount, isPrecise) => volume.Adjust(action, amount, isPrecise),
                },
                contentContainer = new Container
                {
                    Name = "Content",
                    RelativeSizeAxes = Axes.Both,
                },
                controlContainer = new Container
                {
                    AlwaysPresent = true,
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        toolboxContainer = new Container
                        {
                            Alpha = 0,
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = 50, Horizontal = 10 },
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children = new Drawable[]
                            {
                                new LayersToolbox(),
                                new FillFlowContainer
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    AutoSizeAxes = Axes.X,
                                    RelativeSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 10),
                                    Children = new Drawable[]
                                    {
                                        new ProjectToolbox(),
                                        new GroupsToolbox(),
                                    }
                                },
                            }
                        },
                        bottomContainer = new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Margin = new MarginPadding { Bottom = 10 },
                            Padding = new MarginPadding(5),
                        },
                        new Container
                        {
                            Height = 40,
                            Child = new MainMenuBar(),
                            RelativeSizeAxes = Axes.X,
                        }
                    },
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
                spinner = new LoadingSpinner(true)
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Margin = new MarginPadding { Right = 30, Bottom = 30 },
                },
                volume = new VolumeOverlay(),
            };

            showInterface = Session.GetBindable<bool>(EditorSessionStatic.ShowInterface);
            showInterface.BindValueChanged(e => controlContainer.FadeTo(e.NewValue ? 1 : 0, 250, Easing.OutQuint), true);

            Project.ValueChanged += e => Schedule(() => handleProjectChange(e));
            Beatmap.ValueChanged += e => Schedule(() => handleBeatmapChange(e.NewValue));
        }

        private ScheduledDelegate debounce;

        public void Generate()
        {
            debounce?.Cancel();
            debounce = Scheduler.AddDelayed(() => preview?.Generate(), 500);
        }

        protected override void OnPreGenerate()
        {
            spinner.Show();
            output.AddSeparator();
            ScriptLogger.Add($"Generating storyboard for {Beatmap.Value}...");
        }

        protected override void OnPostGenerate()
        {
            spinner.Hide();

            int loaded = Scripts.Count;
            int faulted = Scripts.Where(s => s.Faulted).Count();

            var message = new StringBuilder();
            message.Append("Generation completed");
            message.AppendLine(faulted > 0 ? $@"{"errors".ToQuantity(faulted)}." : ".");
            message.AppendLine($"    {loaded - faulted} script(s) loaded.");

            ScriptLogger.Add(message.ToString());
        }

        private void handleBeatmapChange(WorkingBeatmap working)
        {
            bottomContainer.Clear();
            contentContainer.Clear();

            if (Project.Value is not IFileBackedProject)
                return;

            spinner.Show();

            var beatmap = working.GetPlayableBeatmap(Ruleset.Value);

            LoadComponentAsync(preview = new EditorPreview(beatmap, working.Skin, Ruleset.Value), _ =>
            {
                spinner.Hide();
                toolboxContainer.Show();
                contentContainer.Add(preview);
            });

            LoadComponentAsync(new BottomControls(beatmap), loaded => bottomContainer.Add(loaded));
        }

        private void handleProjectChange(ValueChangedEvent<IProject> project)
        {
            watcher?.Expire();

            if (project.NewValue is IFileBackedProject fileBackedProject)
                AddInternal(watcher = new FileWatcher(fileBackedProject));

            if (project.NewValue is not ICanProvideBeatmap)
            {
                contentContainer?.Clear();
                toolboxContainer.Hide();
            }

            if (project.NewValue is ICanProvideGroups newGroupsProvider)
            {
                newGroupsProvider.Groups.GroupPropertyChanged += e =>
                {
                    if (e == GroupChangeType.Visibility)
                        Generate();
                };

                newGroupsProvider.Groups.Bindable.CollectionChanged += (_, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Move)
                        Generate();
                };
            }

            if (project.NewValue is IGeneratorConfig newConfig)
                newConfig.UseWidescreen.ValueChanged += _ => Generate();
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
                    (Project.Value as IConfigManager)?.Save();
                    return true;

                case PlatformAction.DocumentClose:
                    Project.SetDefault();
                    return true;

                default:
                    return base.OnPressed(e);
            }
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
            double amount = e.ShiftPressed ? 2 : 1;
            Beatmap.Value.Track.Seek(Beatmap.Value.Track.CurrentTime - (500 * amount * direction));
        }
    }
}
