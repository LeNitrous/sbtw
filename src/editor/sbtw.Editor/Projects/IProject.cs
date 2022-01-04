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
    public interface IProject
    {
        string Name { get; }

        string Path { get; }

        List<string> Ignore { get; }

        BindableList<ElementGroupSetting> Groups { get; }

        BindableDictionary<string, IEnumerable<ScriptVariableInfo>> Variables { get; }

        StorageBackedBeatmapSet BeatmapSet { get; }

        DemanglingResourceProvider Resources { get; }

        Storage Files { get; }

        bool Save();
    }
}
