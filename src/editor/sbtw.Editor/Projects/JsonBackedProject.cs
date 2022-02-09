// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Platform;
using osu.Game.Storyboards;
using sbtw.Editor.Assets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonBackedProject : ConfigManager, IFileBackedProject, ICanGenerate<Storyboard>, ICanGenerate<string>
    {
        public string Name { get; }
        public string Path { get; }

        public GroupCollection Groups { get; }
        public BeatmapProvider Beatmaps { get; }
        public Storage Files { get; }
        public Storage BeatmapFiles { get; }
        public ICollection<Asset> Assets { get; }
        public ScriptManager Scripts { get; }

        [JsonProperty(Required = Required.Always)]
        public BindableInt Precision { get; } = new BindableInt();

        [JsonProperty("Groups", Required = Required.Always)]
        private IReadOnlyList<Group> groups { get; set; } = Array.Empty<Group>();

        [JsonProperty("Assets", Required = Required.Always, ItemTypeNameHandling = TypeNameHandling.Auto)]
        private IReadOnlyList<Asset> assets { get; set; } = Array.Empty<Asset>();

        public JsonBackedProject(string path, IEnumerable<Type> scriptLanguageTypes = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (path.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) > -1)
                throw new ArgumentException("Path is not valid", nameof(path));

            if (!path.EndsWith(".sbtw.json"))
                throw new ArgumentException("Path is not a project file", nameof(path));

            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Path = System.IO.Path.GetDirectoryName(path);
            Files = CreateStorage(Path);
            BeatmapFiles = Files.GetStorageForDirectory("Beatmap");

            Load();

            Assets = new AssetCollection(this, assets);
            Groups = new GroupCollection(groups);
            Scripts = new ScriptManager(this, scriptLanguageTypes ?? Enumerable.Empty<Type>());

            (Assets as AssetCollection).CacheChanged += QueueBackgroundSave;
            Groups.GroupPropertyChanged += QueueBackgroundSave;
            Groups.Bindable.CollectionChanged += (_, __) => QueueBackgroundSave();
        }

        public BeatmapProvider GetBeatmapProvider(GameHost host, AudioManager audioManager, AudioMixer audioMixer = null)
            => new StorageBackedBeatmapProvider(host, audioManager, BeatmapFiles, audioMixer);

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
                writer.Write(JsonConvert.SerializeObject(this));
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Scripts?.Dispose();
        }

        Task<Storyboard> ICanGenerate<Storyboard>.GenerateAsync(Dictionary<string, object> resources, ExportTarget? target, bool includeHidden, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        Storyboard ICanGenerate<Storyboard>.Generate(Dictionary<string, object> resources, ExportTarget? target, bool includeHidden)
        {
            throw new NotImplementedException();
        }

        Task<string> ICanGenerate<string>.GenerateAsync(Dictionary<string, object> resources, ExportTarget? target, bool includeHidden, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        string ICanGenerate<string>.Generate(Dictionary<string, object> resources, ExportTarget? target, bool includeHidden)
        {
            throw new NotImplementedException();
        }
    }
}
