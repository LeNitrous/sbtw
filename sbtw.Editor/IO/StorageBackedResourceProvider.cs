// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Audio;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.IO;

namespace sbtw.Editor.IO
{
    public class StorageBackedResourceProvider : IStorageResourceProvider
    {
        public AudioManager AudioManager { get; }
        public IResourceStore<byte[]> Files => Resources;
        public IResourceStore<byte[]> Resources { get; }
        public readonly Storage Storage;

        private readonly GameHost host;

        public StorageBackedResourceProvider(GameHost host, AudioManager audio, Storage storage)
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
