// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Audio;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Database;
using osu.Game.IO;
using FrameworkStorage = osu.Framework.Platform.Storage;

namespace sbtw.Editor.IO.Storage
{
    public class StorageBackedResourceProvider : IStorageResourceProvider
    {
        public AudioManager AudioManager { get; }
        public IResourceStore<byte[]> Files => Resources;
        public IResourceStore<byte[]> Resources { get; }
        public RealmAccess RealmAccess { get; }

        public readonly FrameworkStorage Storage;

        private readonly GameHost host;

        public StorageBackedResourceProvider(GameHost host, AudioManager audio, FrameworkStorage storage)
        {
            this.host = host;
            Storage = storage;
            AudioManager = audio;
            Resources = CreateResourceStore();
        }

        public IResourceStore<TextureUpload> CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
            => host.CreateTextureLoaderStore(underlyingStore);

        protected virtual IResourceStore<byte[]> CreateResourceStore() => new StorageBackedResourceStore(Storage);
    }
}
