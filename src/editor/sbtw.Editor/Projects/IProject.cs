// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;
using osu.Framework.Platform;
using sbtw.Editor.Assets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.IO;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    public interface IProject
    {
        string Name { get; }

        string Path { get; }

        AssetCache Assets { get; }

        BindableList<GroupSetting> Groups { get; }

        BindableList<ScriptGenerationResult> Scripts { get; }

        StorageBackedBeatmapSet BeatmapSet { get; }

        DemanglingResourceProvider Resources { get; }

        Storage Files { get; }

        bool Save();
    }
}
