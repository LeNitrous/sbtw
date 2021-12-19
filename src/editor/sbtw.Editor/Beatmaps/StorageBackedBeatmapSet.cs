// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets;
using sbtw.Editor.IO.Storage;

namespace sbtw.Editor.Beatmaps
{
    public class StorageBackedBeatmapSet
    {
        public BeatmapSetInfo BeatmapSetInfo { get; private set; }

        private readonly Storage storage;
        private readonly RulesetStore rulesets;
        private readonly DemanglingResourceProvider resources;

        public StorageBackedBeatmapSet(Storage storage, GameHost host, AudioManager audio, RulesetStore rulesets)
        {
            this.storage = storage;
            this.rulesets = rulesets;
            resources = new DemanglingResourceProvider(host, audio, storage);
            Refresh();
        }

        public void Refresh()
        {
            BeatmapSetInfo = new BeatmapSetInfo();

            foreach (string filePath in storage.GetFiles(string.Empty))
            {
                if (Path.GetExtension(filePath) == ".osu")
                {
                    using var stream = storage.GetStream(filePath);
                    using var reader = new LineBufferedReader(stream);

                    var beatmap = Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
                    beatmap.BeatmapInfo.Path = filePath;
                    beatmap.BeatmapInfo.Ruleset = rulesets.GetRuleset(beatmap.BeatmapInfo.RulesetID);
                    beatmap.BeatmapInfo.BeatmapSet = BeatmapSetInfo;

                    BeatmapSetInfo.Beatmaps.Add(beatmap.BeatmapInfo);
                    BeatmapSetInfo.Metadata ??= beatmap.Metadata;
                }
            }

            foreach (string filePath in Directory.EnumerateFiles(storage.GetFullPath("."), "*", SearchOption.AllDirectories))
            {
                BeatmapSetInfo.Files.Add(new BeatmapSetFileInfo
                {
                    Filename = filePath.Replace(storage.GetFullPath(".") + "\\", string.Empty).Replace("\\", "/"),
                    FileInfo = new osu.Game.IO.FileInfo { Hash = Path.Combine(new string(' ', 2), "$" + filePath.Replace(storage.GetFullPath(".") + "\\", string.Empty)) }
                });
            }
        }

        public WorkingBeatmap GetWorkingBeatmap(BeatmapInfo beatmapInfo)
        {
            var ownedBeatmapInfo = BeatmapSetInfo.Beatmaps.FirstOrDefault(b => b == beatmapInfo);

            if (ownedBeatmapInfo == null)
                return null;

            return new StorageBackedWorkingBeatmap(resources, ownedBeatmapInfo);
        }

        private class DemanglingResourceProvider : StorageBackedResourceProvider, IBeatmapResourceProvider
        {
            public TextureStore LargeTextureStore { get; }
            public ITrackStore Tracks => AudioManager.GetTrackStore(Resources);

            public DemanglingResourceProvider(GameHost host, AudioManager audio, Storage storage)
                : base(host, audio, storage)
            {
                LargeTextureStore = new LargeTextureStore(host.CreateTextureLoaderStore(Resources));
            }

            protected override IResourceStore<byte[]> CreateResourceStore()
                => new DemanglingResourceStore(Storage);
        }

        private class DemanglingResourceStore : IResourceStore<byte[]>
        {
            private readonly IResourceStore<byte[]> store;

            public DemanglingResourceStore(Storage storage)
            {
                store = new StorageBackedResourceStore(storage);
            }

            public void Dispose() => store.Dispose();
            public byte[] Get(string name) => store.Get(demangle(name));
            public Task<byte[]> GetAsync(string name) => store.GetAsync(demangle(name));
            public Stream GetStream(string name) => store.GetStream(demangle(name));
            public IEnumerable<string> GetAvailableResources() => store.GetAvailableResources();
            private static string demangle(string name) => name.Split('$').Last().Replace("\\", "/");
        }
    }
}
