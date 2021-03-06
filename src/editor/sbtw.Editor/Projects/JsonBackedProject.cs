// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Rulesets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Generators;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Assets;

namespace sbtw.Editor.Projects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonBackedProject : ConfigManager, IFileBackedProject, ICanProvideGroups, ICanProvideLogger, IGeneratorConfig
    {
        public string Name { get; }
        public string Path { get; }
        public string FullPath => System.IO.Path.Combine(Path, System.IO.Path.ChangeExtension(Name, ".sbtw.json"));

        public BeatmapProvider Beatmaps { get; }
        public Storage Files { get; }
        public Storage BeatmapFiles { get; }

        [JsonProperty(Required = Required.Always)]
        public BindableBool UseWidescreen { get; } = new BindableBool(true);

        [JsonProperty]
        public BindableInt PrecisionMove { get; } = new BindableInt { Default = 4, Value = 4, MinValue = 0, MaxValue = 6 };

        [JsonProperty]
        public BindableInt PrecisionScale { get; } = new BindableInt { Default = 4, Value = 4, MinValue = 0, MaxValue = 6 };

        [JsonProperty]
        public BindableInt PrecisionAlpha { get; } = new BindableInt { Default = 4, Value = 4, MinValue = 0, MaxValue = 6 };

        [JsonProperty]
        public BindableInt PrecisionRotation { get; } = new BindableInt { Default = 4, Value = 4, MinValue = 0, MaxValue = 6 };

        [JsonProperty(Required = Required.Always)]
        public HashSet<Asset> Assets { get; } = new HashSet<Asset>();

        [JsonProperty(Required = Required.Always)]
        public GroupCollection Groups { get; } = new GroupCollection();

        public JsonBackedProject(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!path.EndsWith(".sbtw.json"))
                throw new ArgumentException("Path is not a project file", nameof(path));

            Name = System.IO.Path.GetFileNameWithoutExtension(path.Replace(".json", string.Empty));
            Path = System.IO.Path.GetDirectoryName(path);

            Files = CreateStorage(Path);
            BeatmapFiles = Files.GetStorageForDirectory("Beatmap");

            Load();

            Groups.GroupPropertyChanged += _ => QueueBackgroundSave();
            Groups.Bindable.CollectionChanged += (_, __) => QueueBackgroundSave();
            UseWidescreen.ValueChanged += _ => QueueBackgroundSave();
            PrecisionMove.ValueChanged += _ => QueueBackgroundSave();
            PrecisionAlpha.ValueChanged += _ => QueueBackgroundSave();
            PrecisionScale.ValueChanged += _ => QueueBackgroundSave();
            PrecisionRotation.ValueChanged += _ => QueueBackgroundSave();
        }

        public BeatmapProvider GetBeatmapProvider(GameHost host, AudioManager audioManager, RulesetStore rulesets = null, AudioMixer audioMixer = null)
            => new StorageBackedBeatmapProvider(host, audioManager, BeatmapFiles, rulesets, audioMixer);

        protected virtual Storage CreateStorage(string path) => new NativeStorage(path);

        protected override void PerformLoad()
        {
            using var stream = Files.GetStream(System.IO.Path.ChangeExtension(Name, ".sbtw.json"));

            if (stream == null)
                return;

            using var reader = new StreamReader(stream);
            JsonConvert.PopulateObject(reader.ReadToEnd(), this);
        }

        protected override bool PerformSave()
        {
            try
            {
                using var stream = Files.GetStream(System.IO.Path.ChangeExtension(Name, ".sbtw.json"), FileAccess.Write);
                using var writer = new StreamWriter(stream);
                stream.Position = 0;
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Log(object message, LogLevel level = LogLevel.Verbose)
            => EditorBase.ScriptLogger.Add(message.ToString(), level);
    }
}
