// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Screens.Edit;
using osu.Game.Skinning;
using osu.Game.Storyboards;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Configuration;
using sbtw.Editor.Generators;
using sbtw.Editor.Graphics.Containers;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class EditorPreview : CompositeDrawable
    {
        private readonly IBeatmap beatmap;
        private readonly Ruleset ruleset;
        private readonly ISkin skin;
        private EditorDrawableRuleset playfield;
        private Container storyboardMain;
        private Container storyboardOver;
        private Bindable<bool> showPlayfield;

        [Resolved]
        private IBindable<IProject> project { get; set; }

        [Resolved]
        private EditorBase editor { get; set; }

        [Resolved]
        private IBeatmapProvider beatmapProvider { get; set; }

        public EditorPreview(IBeatmap beatmap, ISkin skin, IRulesetInfo ruleset)
        {
            this.skin = skin;
            this.beatmap = beatmap;
            this.ruleset = ruleset.CreateInstance();
        }

        [BackgroundDependencyLoader]
        private void load(EditorSessionStatics statics, EditorClock clock)
        {
            RelativeSizeAxes = Axes.Both;

            AddInternal(new RulesetSkinProvidingContainer(ruleset, beatmap, skin)
            {
                Children = new Drawable[]
                {
                    storyboardMain = new AspectRatioPreservingContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    playfield = new EditorDrawableRuleset(beatmap, ruleset)
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

            Generate();
        }

        public void Generate() => Task.Run(async () =>
        {
            Schedule(() =>
            {
                storyboardMain.Clear();
                storyboardOver.Clear();
            });

            try
            {
                var output = (await editor.Generate(GenerateKind.Storyboard, null, false)) as GeneratorResult<Storyboard>;
                output.Result.BeatmapInfo = beatmap.BeatmapInfo;

                var resources = (beatmapProvider as IBeatmapResourceProvider).Resources;
                var storyboard = new EditorDrawableStoryboard(output.Result, resources);

                Schedule(() => LoadComponentAsync(storyboard, _ =>
                {
                    storyboardMain.Add(storyboard);
                    storyboardOver.Add(storyboard.Children.FirstOrDefault(l => l.Name == "Overlay").CreateProxy());
                }));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load preview.");
            };
        });
    }
}
