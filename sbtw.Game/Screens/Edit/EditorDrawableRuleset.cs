// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Reflection;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;

namespace sbtw.Game.Screens.Edit
{
    public class EditorDrawableRuleset : CompositeDrawable
    {
        private readonly DrawableRuleset drawableRuleset;
        private readonly IBeatmap beatmap;
        private readonly ModAutoplay autoplay;

        public EditorDrawableRuleset(IBeatmap beatmap, DrawableRuleset drawableRuleset, ModAutoplay autoplay)
        {
            this.drawableRuleset = drawableRuleset;
            this.beatmap = beatmap;
            this.autoplay = autoplay;

            RelativeSizeAxes = Axes.Both;
            InternalChild = drawableRuleset;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            drawableRuleset.Playfield.DisplayJudgements.Value = false;
            drawableRuleset.Cursor.Alpha = 0;

            // We need this to have a consistent playback. The gameplay doesn't really matter though!
            drawableRuleset
                .GetType()
                .GetProperty("FrameStablePlayback", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(drawableRuleset, false);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Scheduler.AddOnce(regenerateAutoplay);
        }

        private void regenerateAutoplay()
        {
            drawableRuleset.SetReplayScore(autoplay.CreateReplayScore(beatmap, drawableRuleset.Mods));
        }
    }
}
