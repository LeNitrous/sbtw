// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.IO.Stores;
using osu.Game.Beatmaps;

namespace sbtw.Editor.Beatmaps
{
    public interface IBeatmapProvider
    {
        IReadOnlyList<IBeatmap> Beatmaps { get; }
        IBeatmapSetInfo BeatmapSet { get; }
        IResourceStore<byte[]> Resources { get; }
        IBeatmap GetBeatmap(IBeatmapInfo beatmapInfo);
        WorkingBeatmap GetWorkingBeatmap(IBeatmapInfo beatmapInfo);
    }
}
