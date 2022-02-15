// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using sbtw.Editor.Scripts.Elements;
using SixLabors.ImageSharp;

namespace sbtw.Editor.Scripts.BuiltIns
{
    public class BeatmapBackgroundScript : BuiltInScript
    {
        protected override void Perform(dynamic context)
        {
            IBeatmap beatmap = context.Beatmap;
            string filename = beatmap.BeatmapInfo.Metadata.BackgroundFile;
            double end = beatmap.HitObjects.Max(h => h.GetEndTime());

            int width = 0;
            using (Image image = Image.Load(context.Fetch(Path.Combine("Beatmap", filename))))
                width = image.Width;

            Group group = context.GetGroup("Background");

            ScriptedSprite bg = group.CreateSprite(filename);
            bg.Scale(0, 854.0 / width);
            bg.Fade(0, 1);
            bg.Fade(end, 0);
        }
    }
}
