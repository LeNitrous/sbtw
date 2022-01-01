// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.IO;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    public class DummyProject : IProject
    {
        public string Name => @"No project";
        public string Path => string.Empty;
        public BindableList<string> Groups { get; } = new BindableList<string>();
        public BindableDictionary<string, IEnumerable<ScriptVariableInfo>> Variables { get; } = new BindableDictionary<string, IEnumerable<ScriptVariableInfo>>();
        public StorageBackedBeatmapSet BeatmapSet => null;
        public DemanglingResourceProvider Resources => null;
        public Storage Files => null;

        public bool Save() => true;
    }
}
