// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;

namespace sbtw.Game.Projects
{
    [Serializable]
    public class Project : IProject, IDisposable
    {
        [JsonIgnore]
        public string Name { get; init; }

        [JsonIgnore]
        public string Path { get; init; }

        public bool UseStablePath { get; set; }

        /// <summary>
        /// The path to the beatmap for this project.
        /// </summary>
        /// <remarks>
        /// If <see cref="UseStablePath"/> is true, then it is a path relative to the stable songs folder.
        /// Otherwise, it is the absolute path to the beatmap used for this project.
        /// </remarks>
        [JsonIgnore]
        public string BeatmapPath
        {
            get => UseStablePath ? System.IO.Path.Combine(ProjectHelper.STABLE_PATH, "Songs", beatmapPath) : beatmapPath;
            set => beatmapPath = value;
        }

        [JsonProperty("BeatmapPath")]
        private string beatmapPath;

        [JsonProperty]
        public BindableList<string> Groups { get; private set; } = new BindableList<string>();

        [JsonProperty]
        public Bindable<bool> ShowBeatmapBackground { get; private set; } = new Bindable<bool>();

        [JsonProperty]
        public Bindable<bool> WidescreenStoryboard { get; private set; } = new Bindable<bool>();

        [JsonIgnore]
        public BeatmapSetInfo BeatmapSet => beatmapManager.BeatmapSet;

        [JsonIgnore]
        public IResourceStore<byte[]> Resources => beatmapManager.Resources;

        public event Action<ProjectFileType> FileChanged
        {
            add => fileWatcher.FileChanged += value;
            remove => fileWatcher.FileChanged -= value;
        }

        private ProjectFileWatcher fileWatcher;
        private ProjectBeatmapManager beatmapManager;
        private readonly GameHost host;
        private readonly AudioManager audio;
        private readonly RulesetStore rulesets;

        public Project(GameHost host, AudioManager audio, RulesetStore rulesets)
        {
            this.host = host;
            this.audio = audio;
            this.rulesets = rulesets;
        }

        public void Initialize()
        {
            fileWatcher = new ProjectFileWatcher(this);
            beatmapManager = new ProjectBeatmapManager(this, host, audio, rulesets);
        }

        public void Build()
            => ProjectHelper.Build(Path);

        public void Clean()
            => ProjectHelper.Clean(Path);

        public void Restore()
            => ProjectHelper.Restore(Path);

        public void Save()
            => File.WriteAllText(System.IO.Path.Combine(Path, System.IO.Path.ChangeExtension(Name, ".sbtw.json")), JsonConvert.SerializeObject(this, Formatting.Indented));

        public WorkingBeatmap GetWorkingBeatmap(string version)
            => beatmapManager.GetWorkingBeatmap(version);

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    fileWatcher.Dispose();
                    beatmapManager.Dispose();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
