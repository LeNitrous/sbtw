// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Game.Rulesets;
using sbtw.Editor.Assets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.IO;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    public class Project : IProject
    {
        public string Name { get; }
        public BindableList<Asset> Assets { get; }
        public BindableList<GroupSetting> Groups { get; }
        public BindableList<ScriptGenerationResult> Scripts { get; } = new BindableList<ScriptGenerationResult>();
        public StorageBackedBeatmapSet BeatmapSet { get; }
        public Storage Storage { get; }

        public int Precision
        {
            get => config.Get<int>("precision");
            set => config.Set("precision", value);
        }

        private readonly ProjectConfigManager config;

        public Project(GameHost host, AudioManager audio, RulesetStore rulesets, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!path.EndsWith(".sbtw.json"))
                throw new ArgumentException("File is not a project.");

            Name = Path.GetFileNameWithoutExtension(path);
            Storage = host.GetStorage(Path.GetDirectoryName(path));
            BeatmapSet = new StorageBackedBeatmapSet(new DemanglingResourceProvider(host, audio, Storage.GetStorageForDirectory("Beatmap")), rulesets);

            config = new ProjectConfigManager(this);

            Groups = new BindableList<GroupSetting>(config.Get<IEnumerable<GroupSetting>>("groups"));
            Groups.BindCollectionChanged(handleGroupCollectionChange, true);

            Assets = new BindableList<Asset>(config.Get<IEnumerable<Asset>>("assets"));
            Assets.BindCollectionChanged((_, __) => config.Set("assets", Assets), true);
        }

        public void GenerateAssets(IEnumerable<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var configAssetByHash = Assets.FirstOrDefault(a => a.Hash == asset.Hash);
                var configAssetByPath = Assets.FirstOrDefault(a => a.Path == asset.Path);

                // Asset from config only needs to obtain script
                if (configAssetByHash == configAssetByPath && configAssetByPath.Script == null)
                {
                    configAssetByPath.Register(asset.Script);
                    continue;
                }

                // Find asset with cached hash
                if (configAssetByHash != null)
                {
                    // Directory changed
                    if (File.Exists(configAssetByHash.FullPath) && asset.FullPath != configAssetByHash.FullPath)
                    {
                        File.Delete(configAssetByHash.FullPath);
                        asset.Generate();
                        Assets.Add(asset);
                    }

                    continue;
                }

                // Find asset with path
                if (configAssetByPath != null)
                {
                    // Asset identifier changed
                    if (File.Exists(configAssetByPath.FullPath) && asset.Hash != configAssetByPath.Hash)
                    {
                        File.Delete(configAssetByPath.FullPath);
                        asset.Generate();
                        Assets.Add(asset);
                    }

                    continue;
                }

                // Generate if not exists
                if (!File.Exists(asset.FullPath))
                {
                    asset.Generate();
                    Assets.Add(asset);
                }

                // Add to cache if file exists
                if (File.Exists(asset.FullPath) && configAssetByPath == null && configAssetByHash == null)
                    Assets.Add(asset);
            }
        }

        private void handleGroupCollectionChange(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    registerGroups(args.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    unregisterGroups(args.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    unregisterGroups(args.OldItems);
                    registerGroups(args.NewItems);
                    break;
            }

            config.Set("groups", Groups);
        }

        private void registerGroups(IList items)
        {
            foreach (GroupSetting group in items)
            {
                group.Target.ValueChanged += _ => Save();
                group.Hidden.ValueChanged += _ => Save();
            }
        }

        private static void unregisterGroups(IList items)
        {
            foreach (GroupSetting group in items)
            {
                group.Target.UnbindAll();
                group.Hidden.UnbindAll();
            }
        }

        public bool Save() => config.Save();
    }
}
