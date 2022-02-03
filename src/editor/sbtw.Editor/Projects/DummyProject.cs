// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;
using osu.Framework.Platform;
using sbtw.Editor.Assets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.IO;

namespace sbtw.Editor.Projects
{
    public class DummyProject : IProject
    {
        public string Name => @"No project";
        public string Path => string.Empty;
        public BindableList<ElementGroupSetting> Groups { get; } = new BindableList<ElementGroupSetting>();
        public StorageBackedBeatmapSet BeatmapSet => null;
        public DemanglingResourceProvider Resources => null;
        public Storage Files => null;
        public AssetGenerator Assets => null;

        public bool Save() => true;
    }
}
