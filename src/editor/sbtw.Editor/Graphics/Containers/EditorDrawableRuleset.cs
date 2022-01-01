// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.UI;

namespace sbtw.Editor.Graphics.Containers
{
    public class EditorDrawableRuleset : CompositeDrawable
    {
        private readonly IBeatmap beatmap;
        private readonly Ruleset ruleset;
        private readonly DrawableRuleset drawableRuleset;

        public EditorDrawableRuleset(IBeatmap beatmap, Ruleset ruleset)
        {
            this.beatmap = beatmap;
            this.ruleset = ruleset;
            RelativeSizeAxes = Axes.Both;
            InternalChild = drawableRuleset = ruleset.CreateDrawableRulesetWith(beatmap);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            drawableRuleset.Playfield.DisplayJudgements.Value = false;
            drawableRuleset.Cursor.Alpha = 0;

            // I know reflection is bad buuuuut we need this to have a consistent playback.
            drawableRuleset
                .GetType()
                .GetProperty("FrameStablePlayback", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(drawableRuleset, false);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            var autoplay = ruleset.GetAutoplayMod();
            Scheduler.AddOnce(() => drawableRuleset.SetReplayScore(autoplay.CreateReplayScore(beatmap, drawableRuleset.Mods)));
        }
    }
}
