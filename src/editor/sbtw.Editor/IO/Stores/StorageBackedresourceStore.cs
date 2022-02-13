// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;
using sbtw.Editor.Extensions;

namespace sbtw.Editor.IO.Stores
{
    public class StorageBackedResourceStore : IResourceStore<byte[]>
    {
        private readonly osu.Framework.Platform.Storage storage;

        public StorageBackedResourceStore(osu.Framework.Platform.Storage storage)
        {
            this.storage = storage;
        }

        public byte[] Get(string name)
        {
            using var stream = storage.GetStream(name);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public async Task<byte[]> GetAsync(string name, CancellationToken cancellationToken = default)
        {
            using var stream = storage.GetStream(name);
            byte[] buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer.AsMemory(), cancellationToken);
            return buffer;
        }

        public IEnumerable<string> GetAvailableResources()
            => storage.GetFiles(string.Empty, "*", SearchOption.AllDirectories).ExcludeSystemFileNames();

        public Stream GetStream(string name)
            => storage.GetStream(name);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
