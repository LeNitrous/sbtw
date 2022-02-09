// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Extensions;
using osu.Game.IO;
using osu.Game.Models;
using sbtw.Editor.Extensions;

namespace sbtw.Editor.Beatmaps
{
    public class StorageBackedBeatmapProvider : BeatmapProvider
    {
        public IBeatmapSetInfo BeatmapSet => beatmapSet;
        public IReadOnlyList<IBeatmap> Beatmaps => beatmaps;
        private readonly BeatmapSetInfo beatmapSet;
        private readonly List<IBeatmap> beatmaps = new List<IBeatmap>();
        private HashResolvingResourceStore resources => ((IStorageResourceProvider)this).Resources as HashResolvingResourceStore;

        public StorageBackedBeatmapProvider(GameHost host, AudioManager audio, Storage storage, AudioMixer mixer = null)
            : base(host, audio, new HashResolvingResourceStore(new StorageBackedResourceStore(storage)), mixer)
        {
            beatmapSet = new BeatmapSetInfo();

            foreach (string path in storage.GetFiles(".", "*", SearchOption.AllDirectories))
                beatmapSet.Files.Add(new RealmNamedFileUsage(new RealmFile { Hash = path }, path));

            foreach (string path in storage.GetFiles("."))
            {
                if (!Path.GetExtension(path).EndsWith(".osu"))
                    continue;

                using var stream = storage.GetStream(path);
                using var reader = new LineBufferedReader(stream);

                var beatmap = Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
                beatmap.BeatmapInfo.BeatmapSet = beatmapSet;
                beatmap.BeatmapInfo.Hash = beatmapSet.Files.FirstOrDefault(f => f.Filename == path).File.Hash;

                beatmapSet.Beatmaps.Add(beatmap.BeatmapInfo);
                beatmaps.Add(beatmap);
            }

            resources.Files = beatmapSet.Files;
        }

        public override WorkingBeatmap GetWorkingBeatmap(IBeatmapInfo beatmapInfo)
        {
            var owned = beatmaps.FirstOrDefault(b => b.BeatmapInfo == beatmapInfo);

            if (owned == null)
                return null;

            return new BeatmapProviderWorkingBeatmap(this, owned);
        }

        private class HashResolvingResourceStore : IResourceStore<byte[]>
        {
            public ICollection<RealmNamedFileUsage> Files;
            private readonly IResourceStore<byte[]> store;

            public HashResolvingResourceStore(IResourceStore<byte[]> store)
            {
                this.store = store;
            }

            public void Dispose() => store.Dispose();

            public byte[] Get(string name)
            {
                var file = Files?.FirstOrDefault(file => file.File.Hash == name);
                return store.Get(file.Filename);
            }

            public Task<byte[]> GetAsync(string name, CancellationToken cancellationToken = default)
            {
                var file = Files?.FirstOrDefault(file => file.File.Hash == name);
                return store.GetAsync(file.Filename, cancellationToken);
            }

            public IEnumerable<string> GetAvailableResources() => store.GetAvailableResources();

            public Stream GetStream(string name)
            {
                var file = Files?.FirstOrDefault(file => file.File.GetStoragePath() == name);
                return store.GetStream(file.Filename);
            }
        }
    }
}
