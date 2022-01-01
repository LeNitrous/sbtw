// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Rulesets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.IO;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    public class Project : IProject
    {
        public BindableList<string> Groups { get; } = new BindableList<string>();
        public BindableDictionary<string, IEnumerable<ScriptVariableInfo>> Variables { get; } = new BindableDictionary<string, IEnumerable<ScriptVariableInfo>>();

        [JsonIgnore]
        public string Name { get; }

        [JsonIgnore]
        public string Path { get; }

        [JsonIgnore]
        public StorageBackedBeatmapSet BeatmapSet { get; }

        [JsonIgnore]
        public DemanglingResourceProvider Resources { get; }

        [JsonIgnore]
        public Storage Files { get; }

        public Project(GameHost host, AudioManager audio, RulesetStore rulesets, Storage storage, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Path = storage.GetFullPath(".");
            Files = storage;
            Resources = new DemanglingResourceProvider(host, audio, storage.GetStorageForDirectory("Beatmap"));
            BeatmapSet = new StorageBackedBeatmapSet(Resources, rulesets);
        }

        public bool Save()
        {
            try
            {
                using var stream = Files.GetStream(System.IO.Path.ChangeExtension(Name, ".sbtw.json"), FileAccess.Write);
                using var writer = new StreamWriter(stream);
                stream.SetLength(0);
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to save project");
                return false;
            }
        }
    }
}
