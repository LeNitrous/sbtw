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

namespace sbtw.Editor.Beatmaps
{
    public class FileBasedBeatmapManager : IBeatmapResourceProvider
    {
        private readonly GameHost host;
        private readonly BeatmapSetInfo beatmapSetInfo = new BeatmapSetInfo();

        public IBeatmapSetInfo BeatmapSetInfo => beatmapSetInfo;
        public TextureStore LargeTextureStore { get; }
        public ITrackStore Tracks { get; }
        public AudioManager AudioManager { get; }
        public IResourceStore<byte[]> Files => Resources;
        public IResourceStore<byte[]> Resources { get; }

        public FileBasedBeatmapManager(string path, GameHost host, AudioManager audio, RulesetStore rulesets)
        {
            this.host = host;

            var storage = (host as DesktopGameHost)?.GetStorage(path);

            AudioManager = audio;
            Resources = new DemanglingResourceStore(storage);
            Tracks = audio.GetTrackStore(Resources);
            LargeTextureStore = new LargeTextureStore(host.CreateTextureLoaderStore(Resources));

            foreach (string filePath in storage.GetFiles(string.Empty))
            {
                if (Path.GetExtension(filePath) == ".osu")
                {
                    using var stream = storage.GetStream(filePath);
                    using var reader = new LineBufferedReader(stream);

                    var beatmap = Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
                    beatmap.BeatmapInfo.Path = filePath;
                    beatmap.BeatmapInfo.Ruleset = rulesets.GetRuleset(beatmap.BeatmapInfo.RulesetID);
                    beatmap.BeatmapInfo.BeatmapSet = beatmapSetInfo;

                    beatmapSetInfo.Beatmaps.Add(beatmap.BeatmapInfo);
                    beatmapSetInfo.Metadata ??= beatmap.Metadata;
                }
            }

            foreach (string filePath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                beatmapSetInfo.Files.Add(new BeatmapSetFileInfo
                {
                    Filename = filePath.Replace(path + "\\", string.Empty).Replace("\\", "/"),
                    FileInfo = new osu.Game.IO.FileInfo { Hash = Path.Combine(new string(' ', 2), "$" + filePath.Replace(path + "\\", string.Empty)) }
                });
            }
        }

        public WorkingBeatmap GetWorkingBeatmap(BeatmapInfo beatmapInfo)
        {
            var ownedBeatmapInfo = beatmapSetInfo.Beatmaps.FirstOrDefault(b => b == beatmapInfo);

            if (ownedBeatmapInfo == null)
                return null;

            return new FileBasedWorkingBeatmap(this, ownedBeatmapInfo, AudioManager);
        }

        public IResourceStore<TextureUpload> CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
            => host.CreateTextureLoaderStore(underlyingStore);

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
