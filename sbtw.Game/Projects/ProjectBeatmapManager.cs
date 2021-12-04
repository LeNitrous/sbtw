// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets;

namespace sbtw.Game.Projects
{
    /// <summary>
    /// A <see cref="BeatmapManager"/> shim that completely bypasses database and <see cref="FileStore"/> operations.
    /// </summary>
    public class ProjectBeatmapManager : IBeatmapResourceProvider, IDisposable
    {
        /// <summary>
        /// Gets whether a reimport is needed on the next fetch.
        /// </summary>
        public bool Dirty { get; private set; } = true;

        /// <summary>
        /// Gets the beatmap set for this project.
        /// </summary>
        public BeatmapSetInfo BeatmapSet
        {
            get
            {
                if (!Dirty)
                    return beatmapSet;

                var decoder = new LegacyBeatmapDecoder();
                beatmaps.Clear();

                beatmapSet = new BeatmapSetInfo
                {
                    Beatmaps = new List<BeatmapInfo>(),
                    DateAdded = DateTimeOffset.Now,
                };

                foreach (string path in Directory.EnumerateFiles(project.BeatmapPath, "*.osu", SearchOption.TopDirectoryOnly))
                {
                    using var stream = File.OpenRead(path);
                    using var reader = new LineBufferedReader(stream);

                    var beatmap = decoder.Decode(reader);
                    beatmap.BeatmapInfo.Path = Path.GetFileName(path);
                    beatmap.BeatmapInfo.Ruleset = rulesets.GetRuleset(beatmap.BeatmapInfo.RulesetID);
                    beatmap.BeatmapInfo.BeatmapSet = beatmapSet;

                    beatmapSet.Beatmaps.Add(beatmap.BeatmapInfo);
                    beatmapSet.Metadata ??= beatmap.Metadata;

                    beatmaps.Add(beatmap);
                }

                foreach (string path in Directory.EnumerateFiles(project.BeatmapPath, "*", SearchOption.AllDirectories))
                {
                    beatmapSet.Files.Add(new BeatmapSetFileInfo
                    {
                        Filename = path.Replace(project.BeatmapPath + "\\", string.Empty).Replace("\\", "/"),
                        FileInfo = new osu.Game.IO.FileInfo { Hash = Path.Combine(new string(' ', 2), "$" + path.Replace(project.BeatmapPath + "\\", string.Empty)) }
                    });
                }

                Dirty = false;

                return beatmapSet;
            }
        }

        private BeatmapSetInfo beatmapSet;
        private readonly Project project;
        private readonly GameHost host;
        private readonly AudioManager audio;
        private readonly RulesetStore rulesets;
        private readonly ITrackStore tracks;
        private readonly IResourceStore<byte[]> resources;
        private readonly LargeTextureStore largeTextureStore;
        private readonly List<Beatmap> beatmaps = new List<Beatmap>();

        public ProjectBeatmapManager(Project project, GameHost host, AudioManager audio, RulesetStore rulesets)
        {
            this.host = host;
            this.audio = audio;
            this.project = project;
            this.rulesets = rulesets;

            resources = new ProjectBackedResourceStore(new NativeStorage(project.BeatmapPath, host));
            largeTextureStore = new LargeTextureStore(host.CreateTextureLoaderStore(resources));
            tracks = audio.GetTrackStore(resources);

            project.FileChanged += handleFileEvent;
        }

        public WorkingBeatmap GetWorkingBeatmap(string version)
            => new ProjectWorkingBeatmap(this, getBeatmapInfoByVersion(version), getBeatmapByVersion(version), host, audio);

        private BeatmapInfo getBeatmapInfoByVersion(string version)
            => BeatmapSet.Beatmaps.FirstOrDefault(b => b.Version == version);

        private IBeatmap getBeatmapByVersion(string version)
            => beatmaps.FirstOrDefault(b => b.BeatmapInfo.Version == version);

        private void handleFileEvent(ProjectFileType type)
        {
            switch (type)
            {
                case ProjectFileType.Beatmap:
                case ProjectFileType.Resource:
                case ProjectFileType.Storyboard:
                    Dirty = true;
                    break;
            }
        }

        #region Resources

        TextureStore IBeatmapResourceProvider.LargeTextureStore => largeTextureStore;

        ITrackStore IBeatmapResourceProvider.Tracks => tracks;

        AudioManager IStorageResourceProvider.AudioManager => audio;

        IResourceStore<byte[]> IStorageResourceProvider.Files => resources;

        public IResourceStore<byte[]> Resources => resources;

        IResourceStore<TextureUpload> IStorageResourceProvider.CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
            => host.CreateTextureLoaderStore(underlyingStore);

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    resources.Dispose();
                    largeTextureStore.Dispose();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// A resource store wrapper that intercepts all incoming requests for files and fixes
        /// them from the <see cref="FileInfo.StoragePath"/> to an actual file system path.
        /// </summary>
        private class ProjectBackedResourceStore : IResourceStore<byte[]>
        {
            private readonly IResourceStore<byte[]> store;

            public ProjectBackedResourceStore(NativeStorage storage)
            {
                store = new StorageBackedResourceStore(storage);
            }

            public void Dispose()
                => store.Dispose();

            public byte[] Get(string name)
                => store.Get(name.Split('$').Last().Replace("\\", "/"));

            public Task<byte[]> GetAsync(string name)
                => store.GetAsync(name.Split('$').Last().Replace("\\", "/"));

            public IEnumerable<string> GetAvailableResources()
                => store.GetAvailableResources();

            public Stream GetStream(string name)
                => store.GetStream(name.Split('$').Last().Replace("\\", "/"));
        }
    }
}
