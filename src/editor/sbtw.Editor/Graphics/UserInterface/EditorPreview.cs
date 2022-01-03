// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Play;
using osu.Game.Storyboards;
using osuTK;
using osuTK.Input;
using sbtw.Editor.Configuration;
using sbtw.Editor.Graphics.Containers;
using sbtw.Editor.Languages;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Graphics.UserInterface
{
    [Cached(typeof(IBeatSnapProvider))]
    [Cached(typeof(ISamplePlaybackDisabler))]
    public class EditorPreview : CompositeDrawable, IBeatSnapProvider, ISamplePlaybackDisabler
    {
        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();
        private readonly Bindable<bool> samplePlaybackDisabled = new Bindable<bool>();

        private EditorClock clock;
        private EditorBeatmap editorBeatmap;

        private EditorDrawableRuleset playfield;
        private Container storyboardMain;
        private Container storyboardOver;
        private Bindable<bool> showPlayfield;

        [Resolved]
        private Bindable<IProject> project { get; set; }

        [Resolved]
        private Bindable<WorkingBeatmap> beatmap { get; set; }

        [Resolved]
        private LanguageStore languages { get; set; }

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load(Bindable<EditorClock> clockBindable, Bindable<EditorBeatmap> beatmapBindable, EditorSessionStatics statics, Bindable<RulesetInfo> rulesetInfo)
        {
            RelativeSizeAxes = Axes.Both;

            if (beatmap.Value is DummyWorkingBeatmap)
                return;

            IBeatmap playableBeatmap;

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

            AddInternal(new EditorSkinProvidingContainer(editorBeatmap)
            {
                Children = new Drawable[]
                {
                    new AspectRatioPreservingContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            new BeatmapBackground(beatmap.Value),
                            storyboardMain = new Container { RelativeSizeAxes = Axes.Both },
                        }
                    },
                    playfield = new EditorDrawableRuleset(playableBeatmap, rulesetInfo.Value.CreateInstance())
                    {
                        Clock = clock,
                        ProcessCustomClock = false,
                    },
                    storyboardOver = new AspectRatioPreservingContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                }
            });

            showPlayfield = statics.GetBindable<bool>(EditorSessionStatic.ShowPlayfield);
            showPlayfield.BindValueChanged(e => playfield.Alpha = e.NewValue ? 1 : 0, true);

            clockBindable.Value = clock;
            beatmapBindable.Value = editorBeatmap;
        }

        public void SetStoryboard(Storyboard storyboard, IResourceStore<byte[]> resources)
        {
            storyboardMain.Clear();
            storyboardOver.Clear();

            LoadComponentAsync(new EditorDrawableStoryboard(storyboard, resources), loaded =>
            {
                storyboardMain.Add(loaded);
                storyboardOver.Add(loaded.Children.FirstOrDefault(l => l.Name == "Overlay").CreateProxy());
            });
        }

        public void Seek(double time) => clock?.Seek(time);

        public void Start() => clock?.Start();

        public void Stop() => clock?.Stop();

        protected override void Update()
        {
            base.Update();
            if (beatmap.Value.Track.IsAlive)
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

        private class AspectRatioPreservingContainer : Container
        {
            protected override Vector2 DrawScale => new Vector2(Parent.DrawHeight / 480);

            public AspectRatioPreservingContainer()
            {
                Size = new Vector2(854, 480);
            }
        }

        int IBeatSnapProvider.BeatDivisor => beatDivisor.Value;
        double IBeatSnapProvider.SnapTime(double time, double? referenceTime) => editorBeatmap.SnapTime(time, referenceTime);
        double IBeatSnapProvider.GetBeatLengthAtTime(double referenceTime) => editorBeatmap.GetBeatLengthAtTime(referenceTime);
        IBindable<bool> ISamplePlaybackDisabler.SamplePlaybackDisabled => samplePlaybackDisabled;
    }
}
