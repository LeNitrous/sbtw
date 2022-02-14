// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Database;
using osu.Game.IO;
using osu.Game.Models;
using osu.Game.Rulesets;
using sbtw.Editor.IO.Stores;

namespace sbtw.Editor.Beatmaps
{
    public abstract class BeatmapProvider : IBeatmapResourceProvider, IBeatmapProvider
    {
        public IBeatmapSetInfo BeatmapSet { get; }
        public IReadOnlyList<IBeatmap> Beatmaps { get; }
        public IResourceStore<byte[]> Resources { get; }

        private readonly AudioManager audio;
        private readonly GameHost host;
        private readonly LargeTextureStore largeTextureStore;
        private readonly ITrackStore tracks;
        private readonly HashResolvingResourceStore resources;

        protected BeatmapProvider(GameHost host, AudioManager audio, IResourceStore<byte[]> store, RulesetStore rulesets, AudioMixer mixer = null)
        {
            this.host = host;
            this.audio = audio;

            var files = store.GetAvailableResources().Select(p => p.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            var beatmaps = new List<IBeatmap>();
            var beatmapSet = new BeatmapSetInfo();

            foreach (string path in files)
                beatmapSet.Files.Add(new RealmNamedFileUsage(new RealmFile { Hash = path }, path));

            foreach (string path in files.Where(p => p.EndsWith(".osu")))
            {
                using var stream = store.GetStream(path);
                using var reader = new LineBufferedReader(stream);

                var beatmap = Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
                beatmap.BeatmapInfo.BeatmapSet = beatmapSet;
                beatmap.BeatmapInfo.Ruleset = rulesets?.GetRuleset(beatmap.BeatmapInfo.Ruleset.OnlineID);
                beatmap.BeatmapInfo.Hash = beatmapSet.Files.FirstOrDefault(f => f.Filename == path).File.Hash;

                beatmapSet.Beatmaps.Add(beatmap.BeatmapInfo);
                beatmaps.Add(beatmap);
            }

            Beatmaps = beatmaps;
            BeatmapSet = beatmapSet;
            resources = new HashResolvingResourceStore(BeatmapSet.Files, store);
            Resources = store;

            tracks = audio.GetTrackStore(resources, mixer);
            largeTextureStore = new LargeTextureStore(host.CreateTextureLoaderStore(resources));
        }

        public WorkingBeatmap GetWorkingBeatmap(IBeatmapInfo beatmapInfo)
        {
            var owned = Beatmaps.FirstOrDefault(b => b.BeatmapInfo == beatmapInfo);

            if (owned == null)
                return null;

            return new BeatmapProviderWorkingBeatmap(this, owned);
        }

        public IBeatmap GetBeatmap(IBeatmapInfo beatmapInfo)
            => Beatmaps.FirstOrDefault(b => b.BeatmapInfo == beatmapInfo);

        IResourceStore<TextureUpload> IStorageResourceProvider.CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
            => host.CreateTextureLoaderStore(underlyingStore);

        ITrackStore IBeatmapResourceProvider.Tracks => tracks;
        TextureStore IBeatmapResourceProvider.LargeTextureStore => largeTextureStore;
        IResourceStore<byte[]> IStorageResourceProvider.Files => resources;
        IResourceStore<byte[]> IStorageResourceProvider.Resources => resources;
        AudioManager IStorageResourceProvider.AudioManager => audio;
        RealmAccess IStorageResourceProvider.RealmAccess { get; }
    }
}
