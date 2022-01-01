// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using sbtw.Editor.IO.Storage;
using sbtw.Editor.IO.Stores;

namespace sbtw.Editor.IO
{
    public class DemanglingResourceProvider : StorageBackedResourceProvider, IBeatmapResourceProvider
    {
        public TextureStore LargeTextureStore { get; }
        public ITrackStore Tracks => AudioManager.GetTrackStore(Resources);

        public DemanglingResourceProvider(GameHost host, AudioManager audio, osu.Framework.Platform.Storage storage)
            : base(host, audio, storage)
        {
            LargeTextureStore = new LargeTextureStore(host.CreateTextureLoaderStore(Resources));
        }

        protected override IResourceStore<byte[]> CreateResourceStore() => new DemanglingResourceStore(Storage);
    }
}
