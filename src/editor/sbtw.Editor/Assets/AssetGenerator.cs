// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sbtw.Editor.Assets
{
    public class AssetGenerator
    {
        public IReadOnlyList<Asset> Cache => cache;

        private readonly List<Asset> cache = new List<Asset>();

        public AssetGenerator(IEnumerable<Asset> cache)
        {
            if (cache != null)
                this.cache.AddRange(cache);
        }

        public void Generate(IEnumerable<Asset> assets)
        {
            cache.Clear();

            foreach (var asset in assets)
            {
                // Find asset with cached hash
                var configAssetByHash = cache.FirstOrDefault(a => a.Hash == asset.Hash);
                if (configAssetByHash != null)
                {
                    // Directory changed
                    if (File.Exists(configAssetByHash.FullPath) && asset.FullPath != configAssetByHash.FullPath)
                    {
                        File.Delete(configAssetByHash.FullPath);
                        asset.Generate();
                        cache.Add(asset);
                    }

                    continue;
                }

                // Find asset with path
                var configAssetByPath = cache.FirstOrDefault(a => a.FullPath == asset.FullPath);
                if (configAssetByPath != null)
                {
                    // Asset identifier changed
                    if (File.Exists(configAssetByPath.FullPath) && asset.Hash != configAssetByPath.Hash)
                    {
                        File.Delete(configAssetByPath.FullPath);
                        asset.Generate();
                        cache.Add(asset);
                    }

                    continue;
                }

                // Generate if not exists
                if (!File.Exists(asset.FullPath))
                {
                    asset.Generate();
                    cache.Add(asset);
                }

                // Add to cache if file exists
                if (File.Exists(asset.FullPath) && configAssetByPath == null && configAssetByHash == null)
                    cache.Add(asset);
            }
        }
    }
}
