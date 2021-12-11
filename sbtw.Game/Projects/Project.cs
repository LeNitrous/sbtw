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
using sbtw.Game.Projects.Generators;
using sbtw.Game.Utils;

namespace sbtw.Game.Projects
{
    [Serializable]
    public class Project : IProject, IDisposable
    {
        [JsonIgnore]
        public string Name { get; private set; }

        [JsonIgnore]
        public string Path { get; private set; }

        public bool UseStablePath { get; set; }

        [JsonIgnore]
        public Storage Storage { get; }

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
            get => UseStablePath ? System.IO.Path.Combine(StableHelper.STABLE_PATH, "Songs", beatmapPath) : beatmapPath;
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
        public IBeatmapSetInfo BeatmapSet => beatmapManager.BeatmapSet;

        [JsonIgnore]
        public IResourceStore<byte[]> Resources => beatmapManager.Resources;

        [JsonIgnore]
        public bool IsMsBuildProject => Type != ProjectTemplateType.Freeform;

        [JsonIgnore]
        public readonly ProjectTemplateType Type;

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
        private readonly ProjectGenerator generator;

        public Project(string name, string path, GameHost host, AudioManager audio, RulesetStore rulesets, IResourceStore<byte[]> resources, ProjectTemplateType type)
        {
            Name = name;
            Path = path;
            Type = type;
            Storage = (host as DesktopGameHost)?.GetStorage(path);

            this.host = host;
            this.audio = audio;
            this.rulesets = rulesets;

            switch (type)
            {
                case ProjectTemplateType.CSharp:
                    generator = new CSharpProjectGenerator(this, resources);
                    break;

                case ProjectTemplateType.VisualBasic:
                    generator = new VisualBasicProjectGenerator(this, resources);
                    break;

                case ProjectTemplateType.Freeform:
                    generator = new ProjectGenerator(this, resources);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public void Initialize(bool generateProjectFiles = false)
        {
            fileWatcher = new ProjectFileWatcher(this);
            beatmapManager = new ProjectBeatmapManager(this, host, audio, rulesets);

            if (generateProjectFiles)
                generator.Generate();
        }

        public void Build()
        {
            if (IsMsBuildProject)
                NetDriverHelper.Build(Path);
        }

        public void Clean()
        {
            if (IsMsBuildProject)
                NetDriverHelper.Clean(Path);
        }

        public void Restore()
        {
            if (IsMsBuildProject)
                NetDriverHelper.Restore(Path);
        }

        public void Save()
        {
            using var stream = Storage.GetStream(System.IO.Path.ChangeExtension(Name, ".sbtw.json"), FileAccess.Write, FileMode.OpenOrCreate);
            using var writer = new StreamWriter(stream);
            stream.SetLength(0);
            writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void GenerateScript(string name)
            => (generator as IMsBuildProjectGenerator)?.GenerateScript(name);

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
