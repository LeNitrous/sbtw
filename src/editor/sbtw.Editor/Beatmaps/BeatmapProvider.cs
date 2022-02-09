// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.IO;

namespace sbtw.Editor.Beatmaps
{
    public abstract class BeatmapProvider : IStorageResourceProvider, IBeatmapResourceProvider
    {
        private readonly AudioManager audio;
        private readonly GameHost host;
        private readonly IResourceStore<byte[]> store;
        private readonly LargeTextureStore largeTextureStore;
        private readonly ITrackStore tracks;

        protected BeatmapProvider(GameHost host, AudioManager audio, IResourceStore<byte[]> store, AudioMixer mixer = null)
        {
            this.host = host;
            this.audio = audio;
            this.store = store;
            tracks = audio.GetTrackStore(store, mixer);
            largeTextureStore = new LargeTextureStore(host.CreateTextureLoaderStore(store));
        }

        public abstract WorkingBeatmap GetWorkingBeatmap(IBeatmapInfo beatmapInfo);

        IResourceStore<TextureUpload> IStorageResourceProvider.CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
            => host.CreateTextureLoaderStore(underlyingStore);

        ITrackStore IBeatmapResourceProvider.Tracks => tracks;
        TextureStore IBeatmapResourceProvider.LargeTextureStore => largeTextureStore;
        IResourceStore<byte[]> IStorageResourceProvider.Resources => store;
        IResourceStore<byte[]> IStorageResourceProvider.Files => store;
        AudioManager IStorageResourceProvider.AudioManager => audio;
        RealmAccess IStorageResourceProvider.RealmAccess { get => throw new NotSupportedException(); }
    }
}
