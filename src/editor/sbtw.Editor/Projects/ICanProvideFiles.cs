// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Platform;
using sbtw.Editor.IO.Storage;

namespace sbtw.Editor.Projects
{
    /// <summary>
    /// Denotes capability of providing storage access.
    /// </summary>
    public interface ICanProvideFiles
    {
        ReferenceTrackingStorage Files { get; }
        IReadOnlySet<string> References => Files.References;
        Storage BeatmapFiles { get; }
    }
}
