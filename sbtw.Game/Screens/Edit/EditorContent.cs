// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Play;
using osuTK.Input;
using sbtw.Game.Projects;
using sbtw.Game.Screens.Edit.Menus;
using sbtw.Game.Screens.Edit.Toolbox;
using sbtw.Game.Scripting;

namespace sbtw.Game.Screens.Edit
{
    [Cached(typeof(IBeatSnapProvider))]
    [Cached(typeof(ISamplePlaybackDisabler))]
    public class EditorContent : CompositeDrawable, IBeatSnapProvider, ISamplePlaybackDisabler
    {
        public Container Controls { get; private set; }

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private LoadingSpinner spinner;
        private EditorClock clock;
        private EditorBeatmap editorBeatmap;
        private IBeatmap playableBeatmap;
        private BeatmapBackground background;
        private ViewToolboxGroup viewToolbox;
        private EditorDrawableRuleset playfield;
        private Container backgroundContent;
        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();
        private readonly Bindable<bool> samplePlaybackDisabled = new Bindable<bool>();

        [Resolved]
        private Bindable<WorkingBeatmap> beatmap { get; set; }

        [Resolved]
        private Bindable<IProject> project { get; set; }

        [Resolved]
        private Bindable<RulesetInfo> rulesetInfo { get; set; }

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        [Resolved]
        private SBTWOutputManager logger { get; set; }

        [Resolved]
        private ChatOverlay chat { get; set; }

        [Resolved]
        private V8ScriptEngine jsScriptEngine { get; set; }

        [Cached(typeof(IBindable<EditorDrawableStoryboard>))]
        private readonly Bindable<EditorDrawableStoryboard> storyboard = new Bindable<EditorDrawableStoryboard>();

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            if (beatmap.Value is DummyWorkingBeatmap)
                return;

            try
            {
                playableBeatmap = beatmap.Value.GetPlayableBeatmap(beatmap.Value.BeatmapInfo.Ruleset);
            }
            catch (Exception)
            {
                return;
            }

            dependencies.Cache(beatDivisor);

            AddInternal(clock = new EditorClock(playableBeatmap, beatDivisor) { IsCoupled = false });
            clock.ChangeSource(beatmap.Value.Track);
            clock.SeekingOrStopped.BindValueChanged(e => samplePlaybackDisabled.Value = e.NewValue, true);
            dependencies.CacheAs(clock);

            AddInternal(editorBeatmap = new EditorBeatmap(playableBeatmap, beatmap.Value.Skin, beatmap.Value.BeatmapInfo));
            dependencies.CacheAs(editorBeatmap);

            var ruleset = rulesetInfo.Value.CreateInstance();
            AddRangeInternal(new Drawable[]
            {
                backgroundContent = new AspectRatioPreservingContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        background = new BeatmapBackground(beatmap.Value),
                    },
                },
                new EditorSkinProvidingContainer(editorBeatmap)
                {
                    Child = playfield = new EditorDrawableRuleset(playableBeatmap, ruleset.CreateDrawableRulesetWith(playableBeatmap), ruleset.GetAutoplayMod())
                    {
                        Clock = clock,
                        ProcessCustomClock = false,
                    },
                },
                Controls = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = 50, Bottom = 90, Horizontal = 10 },
                            Children = new Drawable[]
                            {
                                viewToolbox = new ViewToolboxGroup
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                }
                            },
                        },
                        new BottomMenuBar(),
                    }
                },
                spinner = new LoadingSpinner(true)
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Margin = new MarginPadding { Right = 30, Bottom = 30 },
                }
            });

            project.Value.ShowBeatmapBackground.BindValueChanged(e => background.FadeTo(e.NewValue ? 1 : 0, 200, Easing.OutQuint));
            viewToolbox.PlayfieldVisibility.BindValueChanged(e => playfield.Alpha = e.NewValue ? 1 : 0);

            GenerateStoryboard();
        }

        protected override void Update()
        {
            base.Update();
            clock?.ProcessFrame();
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

        private CancellationTokenSource generatorCancellationToken;

        public void GenerateStoryboard()
        {
            if (project.Value is not Project workingProject)
                return;

            spinner.Show();
            workingProject.Build(generateStoryboard);
        }

        private void generateStoryboard()
        {
            if (project.Value is not Project workingProject)
                return;

            generatorCancellationToken?.Cancel();
            generatorCancellationToken = new CancellationTokenSource();

            Task.Run(async () =>
            {
                using var generator = new StoryboardGenerator(workingProject, beatmap.Value.BeatmapInfo, jsScriptEngine);
                var generated = await generator.GenerateAsync(generatorCancellationToken.Token);

                Schedule(() =>
                {
                    LoadComponentAsync(new EditorDrawableStoryboard(generated), loaded =>
                        {
                            storyboard.Value?.Expire();
                            backgroundContent.Add(storyboard.Value = loaded);

                            spinner.Hide();
                            generatorCancellationToken = null;
                        }, generatorCancellationToken.Token);
                });

            }, generatorCancellationToken.Token).ContinueWith(task =>
            {
                Schedule(() =>
                {
                    spinner.Hide();

                    if (task.Exception.InnerExceptions.FirstOrDefault() is TaskCanceledException)
                        return;

                    notifications.Post(new SimpleErrorNotification
                    {
                        Text = @"A script has generated an error. See output for details.",
                        Activated = () =>
                        {
                            chat.Show();
                            return true;
                        }
                    });

                    foreach (var exception in task.Exception.InnerExceptions)
                    {
                        if (exception is ScriptEngineException jsException)
                            logger.Post(jsException.ErrorDetails, LogLevel.Error);
                        else if (exception is PythonExecutionException pyException)
                            logger.Post(exception.Message, LogLevel.Error);
                        else
                            logger.Post(exception.ToString(), LogLevel.Error);
                    }
                });
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void seek(UIEvent e, int direction)
        {
            double amount = e.ShiftPressed ? 4 : 1;

            bool trackPlaying = clock.IsRunning;

            if (trackPlaying)
                amount *= beatDivisor.Value;

            if (direction < 1)
                clock.SeekBackward(!trackPlaying, amount);
            else
                clock.SeekForward(!trackPlaying, amount);
        }

        int IBeatSnapProvider.BeatDivisor => beatDivisor.Value;
        double IBeatSnapProvider.SnapTime(double time, double? referenceTime) => editorBeatmap.SnapTime(time, referenceTime);
        double IBeatSnapProvider.GetBeatLengthAtTime(double referenceTime) => editorBeatmap.GetBeatLengthAtTime(referenceTime);
        IBindable<bool> ISamplePlaybackDisabler.SamplePlaybackDisabled => samplePlaybackDisabled;
    }
}
