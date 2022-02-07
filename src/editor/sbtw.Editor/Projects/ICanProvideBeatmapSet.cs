// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Beatmaps;

namespace sbtw.Editor.Projects
{
    /// <summary>
    /// Determines that the implementer can provide beatmapset information.
    /// </summary>
    public interface ICanProvideBeatmapSet
    {
        BeatmapSetProvider BeatmapSet { get; }
    }
}
