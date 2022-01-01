// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;

namespace sbtw.Editor.IO.Stores
{
    public class DemanglingResourceStore : IResourceStore<byte[]>
    {
        private readonly IResourceStore<byte[]> store;

        public DemanglingResourceStore(osu.Framework.Platform.Storage storage)
        {
            store = new StorageBackedResourceStore(storage);
        }

        public byte[] Get(string name) => store.Get(demangle(name));
        public Task<byte[]> GetAsync(string name, CancellationToken token) => store.GetAsync(demangle(name), token);
        public Stream GetStream(string name) => store.GetStream(demangle(name));
        public IEnumerable<string> GetAvailableResources() => store.GetAvailableResources();
        private static string demangle(string name) => name.Split('$').Last().Replace("\\", "/");

        public void Dispose()
        {
            store.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
