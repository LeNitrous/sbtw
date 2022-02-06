// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using sbtw.Editor.Assets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    public interface IProject
    {
        string Name { get; }
        int Precision { get; set; }
        Storage Storage { get; }
        StorageBackedBeatmapSet BeatmapSet { get; }
        BindableList<Asset> Assets { get; }
        BindableList<GroupSetting> Groups { get; }
        BindableList<ScriptGenerationResult> Scripts { get; }
        bool Save();
        void GenerateAssets(IEnumerable<Asset> assets);
    }
}
