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
    public class DummyProject : IProject
    {
        public string Name => @"no project";
        public int Precision { get; set; }
        public Storage Storage => null;
        public StorageBackedBeatmapSet BeatmapSet { get; }
        public BindableList<Asset> Assets { get; }
        public BindableList<GroupSetting> Groups { get; }
        public BindableList<ScriptGenerationResult> Scripts { get; }

        public void GenerateAssets(IEnumerable<Asset> assets)
        {
        }

        public bool Save() => false;
    }
}
