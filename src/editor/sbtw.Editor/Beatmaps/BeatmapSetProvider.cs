// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Beatmaps;

namespace sbtw.Editor.Beatmaps
{
    public abstract class BeatmapSetProvider
    {
        public abstract IWorkingBeatmap GetWorkingBeatmap();
        public abstract IBeatmapSetInfo GetBeatmapSet();
    }
}
