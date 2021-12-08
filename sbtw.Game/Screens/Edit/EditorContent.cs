// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Play;
using osuTK.Input;
using sbtw.Game.Projects;
using sbtw.Game.Screens.Edit.Menus;
using sbtw.Game.Scripting;

namespace sbtw.Game.Screens.Edit
{
    [Cached(typeof(IBeatSnapProvider))]
    [Cached(typeof(ISamplePlaybackDisabler))]
    public class EditorContent : CompositeDrawable, IBeatSnapProvider, ISamplePlaybackDisabler
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private EditorClock clock;
        private EditorBeatmap editorBeatmap;
        private IBeatmap playableBeatmap;
        private BeatmapBackground background;
        private Container storyboard;
        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();
        private readonly Bindable<bool> samplePlaybackDisabled = new Bindable<bool>();

        [Resolved]
        private Bindable<WorkingBeatmap> beatmap { get; set; }

        [Resolved]
        private Bindable<IProject> project { get; set; }

        [Resolved]
        private Bindable<RulesetInfo> rulesetInfo { get; set; }

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
                new AspectRatioPreservingContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        background = new BeatmapBackground(beatmap.Value),
                        storyboard = new Container { RelativeSizeAxes = Axes.Both },
                    },
                },
                new EditorSkinProvidingContainer(editorBeatmap)
                {
                    Child = new EditorDrawableRuleset(playableBeatmap, ruleset.CreateDrawableRulesetWith(playableBeatmap), ruleset.GetAutoplayMod())
                    {
                        Clock = clock,
                        ProcessCustomClock = false,
                    },
                },
                new BottomMenuBar(),
            });

            project.Value.ShowBeatmapBackground.BindValueChanged(e => background.FadeTo(e.NewValue ? 1 : 0, 200, Easing.OutQuint));
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

        public void GenerateStoryboard()
        {
            if (project.Value is not Project usableProject)
                return;

            using var generator = new StoryboardGenerator(usableProject, beatmap.Value.BeatmapInfo);
            storyboard.Child = new EditorDrawableStoryboard(generator.Generate(usableProject.Groups));
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
