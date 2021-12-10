// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Game;
using osu.Game.Graphics.Cursor;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input.Bindings;
using osu.Game.Online.API;
using osu.Game.Online.Chat;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Overlays.Volume;
using osu.Game.Screens;
using Python.Runtime;
using sbtw.Game.Projects;
using sbtw.Game.Screens;
using sbtw.Game.Screens.Edit;
using sbtw.Game.Utils;

namespace sbtw.Game
{
    public abstract class SBTWGame : OsuGameBase, IKeyBindingHandler<GlobalAction>
    {
        protected OsuScreenStack ScreenStack;

        private VolumeOverlay volume;
        private SBTWOutputManager channelManager;
        private DependencyContainer dependencies;
        private NotificationOverlay notifications;
        private ChatOverlay chatOverlay;
        private DummyAPIAccess dummyAPI;
        private BackButton.Receptor receptor;
        private BackButton backButton;

        [Cached]
        private readonly NetProcessListener netProcessListener = new NetProcessListener();

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(SBTWGame).Assembly), "Resources"));

            var projectManager = new ProjectManager(Host, Audio, RulesetStore, BeatmapManager.DefaultBeatmap);
            dependencies.CacheAs(projectManager);
            dependencies.CacheAs<Bindable<IProject>>(new NonNullableBindable<IProject>(projectManager.DefaultProject));

            AddInternal(channelManager = new SBTWOutputManager());
            dependencies.CacheAs<ChannelManager>(channelManager);
            dependencies.CacheAs(channelManager);

            AddInternal(dummyAPI = new DummyAPIAccess());
            dependencies.CacheAs<IAPIProvider>(dummyAPI);
            dependencies.CacheAs(dummyAPI);

            dummyAPI.Login("user", "pass");

            AddInternal(netProcessListener);
            dependencies.CacheAs(netProcessListener);

            dependencies.CacheAs(this);

            var debuggerPoller = new DebuggerPoller();
            AddInternal(debuggerPoller);
            dependencies.CacheAs(debuggerPoller);

            Add(new OsuContextMenuContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new VolumeControlReceptor
                    {
                        RelativeSizeAxes = Axes.Both,
                        ActionRequested = action => volume.Adjust(action),
                        ScrollActionRequested = (action, amount, isPrecise) => volume.Adjust(action, amount, isPrecise),
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            receptor = new BackButton.Receptor(),
                            ScreenStack = new OsuScreenStack
                            {
                                RelativeSizeAxes = Axes.Both,
                            },
                            backButton = new BackButton(receptor)
                            {
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                                Action = () =>
                                {
                                    if (ScreenStack.CurrentScreen is not IOsuScreen currentScreen)
                                        return;

                                    if (!((Drawable)currentScreen).IsLoaded || (currentScreen.AllowBackButton && !currentScreen.OnBackButton()))
                                        ScreenStack.Exit();
                                }
                            }
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            volume = new VolumeOverlay(),
                            notifications = new NotificationOverlay
                            {
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                            },
                            chatOverlay = new ChatOverlay
                            {
                                RelativeSizeAxes = Axes.Both,
                            },
                        }
                    }
                }
            });

            dependencies.CacheAs(notifications);
            dependencies.CacheAs(chatOverlay);

            ScreenStack.ScreenPushed += screenPushed;
            ScreenStack.ScreenExited += screenExited;

            SkinManager.CurrentSkinInfo.Value = SkinManager.DefaultLegacySkin.SkinInfo;

            if (!checkAndReportForDependencies())
                return;
            ScreenStack.Push(new SBTWEditor());
        }

        private bool checkAndReportForDependencies()
        {
            if (NetDriverHelper.HAS_DOTNET)
            {
                channelManager.Post($".NET driver found in {NetDriverHelper.DOTNET_PATH}");
                NetDriverHelper.OnDotNetError += msg => channelManager.Post(msg, LogLevel.Error);
                NetDriverHelper.OnDotNetMessage += msg => channelManager.Post(msg);
            }
            else
            {
                channelManager.Post(".NET driver is not found.", LogLevel.Error);
                notifications.Post(new SimpleErrorNotification
                {
                    Text = "Unable to locate the .NET driver. This results in a degraded experience in the editor.",
                    Icon = FontAwesome.Solid.Bomb,
                });
            }

            if (CodeHelper.EDITORS.Any())
            {
                channelManager.Post($"Code Editors found:\n{string.Join('\n', CodeHelper.EDITORS.Select(e => $"{e.Value} ({e.Key})"))}");
            }
            else
            {
                channelManager.Post("No code editors found.");
                notifications.Post(new SimpleErrorNotification
                {
                    Text = "Unable to locate a Visual Studio Code installation. You will be unable to open projects from the editor.",
                    Icon = FontAwesome.Solid.Bomb,
                });
            }

            if (PythonHelper.HAS_PYTHON)
            {
                channelManager.Post($"Python {PythonEngine.Version.Split(' ').FirstOrDefault()} found in {PythonHelper.PYTHON_PATH}");
            }

            if (!RulesetStore.AvailableRulesets.Any())
            {
                notifications.Post(new SimpleErrorNotification
                {
                    Text = "No rulesets found. Cannot continue to the editor.",
                    Icon = FontAwesome.Solid.Bomb,
                });
                return false;

            }

            return true;
        }

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            switch (e.Action)
            {
                case GlobalAction.ToggleChat:
                    chatOverlay?.ToggleVisibility();
                    return true;

                case GlobalAction.ToggleNotifications:
                    notifications?.ToggleVisibility();
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }

        private void screenChanged(IScreen _, IScreen newScreen)
        {
            if (newScreen is IOsuScreen newOsuScreen)
            {
                if (newOsuScreen.AllowBackButton)
                    backButton.Show();
                else
                    backButton.Hide();
            }
        }

        private void screenPushed(IScreen last, IScreen next)
        {
            screenChanged(last, next);
        }

        private void screenExited(IScreen last, IScreen next)
        {
            screenChanged(last, next);
            if (next == null)
                Exit();
        }

        public void OpenFileDialog(IEnumerable<string> filters, string filterDescription, Action<string> onComplete) => Task.Run(() =>
        {
            string path = OpenFileDialog(filters, filterDescription);
            if (path != null)
                onComplete.Invoke(path);
        });

        public void SaveFileDialog(string filename, IEnumerable<string> filters, string filterDescription, Action<string> onComplete) => Task.Run(() =>
        {
            string path = SaveFileDialog(filename, filters, filterDescription);
            if (path != null)
                onComplete.Invoke(path);
        });

        protected abstract string OpenFileDialog(IEnumerable<string> filters, string filterDescription);

        protected abstract string SaveFileDialog(string filename, IEnumerable<string> filters, string filterDescription);
    }
}
