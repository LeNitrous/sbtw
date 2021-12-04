// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Play;
using sbtw.Game.Projects;
using sbtw.Game.Screens.Edit.Menus;

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
            clock.SeekingOrStopped.BindValueChanged(e => samplePlaybackDisabled.Value = e.NewValue);
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

        int IBeatSnapProvider.BeatDivisor => beatDivisor.Value;
        double IBeatSnapProvider.SnapTime(double time, double? referenceTime) => editorBeatmap.SnapTime(time, referenceTime);
        double IBeatSnapProvider.GetBeatLengthAtTime(double referenceTime) => editorBeatmap.GetBeatLengthAtTime(referenceTime);
        IBindable<bool> ISamplePlaybackDisabler.SamplePlaybackDisabled => samplePlaybackDisabled;
    }
}
