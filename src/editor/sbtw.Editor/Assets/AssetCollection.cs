// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;

namespace sbtw.Editor.Assets
{
    /// <summary>
    /// Manages asset generation.
    /// </summary>
    public class AssetCollection : IList<Asset>, ICollection<Asset>, IEnumerable<Asset>, IEnumerable
    {
        private readonly List<Asset> cache = new List<Asset>();
        private readonly Storage storage;

        public int Count => cache.Count;

        public bool IsReadOnly => false;

        public Asset this[int index]
        {
            get => cache[index];
            set => update(new List<Asset>(cache) { [index] = value });
        }

        public AssetCollection(Storage storage, IEnumerable<Asset> initial = null)
        {
            this.storage = storage;

            if (initial != null)
                update(initial);
        }

        /// <summary>
        /// Adds a new asset to cache.
        /// </summary>
        /// <param name="asset">The asset to add.</param>
        public void Add(Asset asset)
            => AddRange(new[] { asset });

        /// <summary>
        /// Removes an asset from cache.
        /// </summary>
        /// <param name="asset">The asset to remove.</param>
        /// /// <returns>Whether there were any assets removed.</returns>
        public bool Remove(Asset asset)
            => RemoveRange(new[] { asset });

        /// <summary>
        /// Adds a range of assets to cache.
        /// </summary>
        /// <param name="assets">Assets to add.</param>
        public void AddRange(IEnumerable<Asset> assets)
            => update(cache.Concat(assets));

        /// <summary>
        /// Removes a range of assets from cache.
        /// </summary>
        /// <param name="assets">Assets to remove.</param>
        /// <returns>Whether there were any assets removed.</returns>
        public bool RemoveRange(IEnumerable<Asset> assets)
        {
            int initial = cache.Count;
            update(cache.Except(assets));
            return initial == cache.Count;
        }

        /// <summary>
        /// Replaces the cache with a range of assets.
        /// </summary>
        /// <param name="assets">Assets to apply.</param>
        public void Apply(IEnumerable<Asset> assets)
            => update(assets);

        /// <summary>
        /// Removes all assets from cache.
        /// </summary>
        public void Clear()
            => update(Enumerable.Empty<Asset>());

        /// <summary>
        /// Updates the cache and creates or deletes assets whenever necessary.
        /// </summary>
        /// <remarks>
        /// Assets are handled by the given conditions:
        /// <br/>
        /// - If there is no asset that matches by path and properties then a new
        /// asset is created and is added to the cache.
        /// <br/>
        /// - If there is an asset that matches by path then the asset will be
        /// regenerated by replacing the original with the new asset.
        /// <br/>
        /// - If there is an asset that matches by property then the asset will
        /// be moved if the file exists or regenerated if the original file is
        /// not found.
        /// <br/>
        /// - If there is an asset that matches by both path and properties then
        /// nothing else will happen.
        /// <br/>
        /// Lastly, all assets with no references will be discarded from the cache.
        /// </remarks>
        private void update(IEnumerable<Asset> assets)
        {
            if (assets == null)
                throw new ArgumentNullException(nameof(assets));

            foreach (var asset in cache)
                asset.ReferenceCount = 0;

            foreach (var asset in assets)
            {
                var assetByPath = cache.FirstOrDefault(a => a.Path == asset.Path);
                var assetByProp = cache.FirstOrDefault(a => a.Equals(asset));

                if (assetByPath != null && assetByProp != null)
                {
                    assetByPath.ReferenceCount++;
                    continue;
                }

                if (assetByPath == null && assetByProp != null)
                {
                    asset.ReferenceCount = assetByProp.ReferenceCount;
                    assetByProp.ReferenceCount = 0;
                    cache.Add(asset);
                    continue;
                }

                if (assetByPath != null && assetByProp == null)
                {
                    asset.ReferenceCount = assetByPath.ReferenceCount;
                    assetByPath.ReferenceCount = 0;
                    cache.Add(asset);
                    continue;
                }

                if (assetByPath == null && assetByProp == null)
                {
                    asset.ReferenceCount++;
                    cache.Add(asset);
                }
            }

            queueBackgroundUpdate();
        }

        private int lastUpdate;
        private readonly object updateLock = new object();

        private void queueBackgroundUpdate()
        {
            int current = Interlocked.Increment(ref lastUpdate);

            Task.Delay(100).ContinueWith(_ =>
            {
                if (current != lastUpdate)
                    return;

                lock (updateLock)
                {
                    Interlocked.Increment(ref lastUpdate);

                    foreach (var asset in cache.ToArray())
                    {
                        if (asset.ReferenceCount > 0)
                        {
                            if (!File.Exists(storage.GetFullPath(asset.Path)))
                                asset.Generate(storage);
                        }
                        else
                        {
                            if (File.Exists(storage.GetFullPath(asset.Path)))
                                File.Delete(storage.GetFullPath(asset.Path));

                            cache.Remove(asset);
                        }
                    }
                }
            });
        }

        public bool Contains(Asset asset)
            => cache.Contains(asset);

        public void CopyTo(Asset[] array, int arrayIndex)
            => cache.CopyTo(array, arrayIndex);

        public IEnumerator<Asset> GetEnumerator()
            => cache.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => cache.GetEnumerator();

        public int IndexOf(Asset item)
            => cache.IndexOf(item);

        public void Insert(int index, Asset item)
        {
            var copy = new List<Asset>(cache);
            copy.Insert(index, item);
            update(copy);
        }

        public void RemoveAt(int index)
            => Remove(cache[index]);
    }
}
