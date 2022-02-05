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
using sbtw.Editor.Assets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.IO;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    public class Project : IProject, IDisposable
    {
        [JsonProperty("precision")]
        public BindableInt Precision { get; } = new BindableInt
        {
            MinValue = 1,
            MaxValue = 6,
            Default = 2,
        };

        [JsonProperty]
        private IReadOnlyList<Asset> assets
        {
            get => Assets?.Cache ?? Array.Empty<Asset>();
            set => Assets = new AssetCache(value);
        }

        [JsonIgnore]
        public string Path => Files.GetFullPath(".");

        [JsonProperty("groups")]
        public BindableList<GroupSetting> Groups { get; } = new BindableList<GroupSetting>();

        [JsonIgnore]
        public BindableList<ScriptGenerationResult> Scripts { get; } = new BindableList<ScriptGenerationResult>();

        [JsonIgnore]
        public StorageBackedBeatmapSet BeatmapSet { get; }

        [JsonIgnore]
        public DemanglingResourceProvider Resources { get; }

        [JsonIgnore]
        public Storage Files { get; }

        [JsonIgnore]
        public AssetCache Assets { get; private set; }

        private readonly Stream stream;

        public Project(GameHost host, AudioManager audio, RulesetStore rulesets, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!path.EndsWith(".sbtw.json"))
                throw new ArgumentException("File is not a project.");

            Files = host.GetStorage(System.IO.Path.GetDirectoryName(path));
            Resources = new DemanglingResourceProvider(host, audio, Files.GetStorageForDirectory("Beatmap"));
            BeatmapSet = new StorageBackedBeatmapSet(Resources, rulesets);

            stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var reader = new StreamReader(stream, leaveOpen: true);
            JsonConvert.PopulateObject(reader.ReadToEnd(), this, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        public bool Save()
        {
            try
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                stream.SetLength(0);
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to save project");
                return false;
            }
        }

        public void Dispose()
        {
            stream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
