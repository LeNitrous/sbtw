// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;
using osu.Game.Database;
using osu.Game.Extensions;

namespace sbtw.Editor.IO.Stores
{
    public class HashResolvingResourceStore : IResourceStore<byte[]>
    {
        private readonly IEnumerable<INamedFileUsage> files;
        private readonly IResourceStore<byte[]> store;

        public HashResolvingResourceStore(IEnumerable<INamedFileUsage> files, IResourceStore<byte[]> store)
        {
            this.files = files;
            this.store = store;
        }

        public byte[] Get(string name) => store.Get(getRealPath(name));
        public Task<byte[]> GetAsync(string name, CancellationToken token = default) => store.GetAsync(getRealPath(name), token);
        public IEnumerable<string> GetAvailableResources() => store.GetAvailableResources();
        public Stream GetStream(string name) => store.GetStream(getRealPath(name));
        private string getRealPath(string name)
            => files?.FirstOrDefault(file => file.File.GetStoragePath() == name)?.Filename ?? string.Empty;

        public void Dispose()
        {
            store.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
