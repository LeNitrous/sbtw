// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;

namespace sbtw.Game.Projects
{
    public class Project : IDisposable
    {
        /// <summary>
        /// The name of this project.
        /// </summary>
        public string Name => config.Name;

        /// <summary>
        /// The absolute path to the project.
        /// </summary>
        public string Path => config.Path;

        /// <summary>
        /// The path to the beatmap for this project.
        /// </summary>
        /// <remarks>
        /// If <see cref="UseStablePath"/> is true, then it is a path relative to the stable songs folder.
        /// Otherwise, it is the absolute path to the beatmap used for this project.
        /// </remarks>
        public string BeatmapPath
        {
            get => UseStablePath ? System.IO.Path.Combine(ProjectHelper.STABLE_PATH, "Songs", beatmapPath) : beatmapPath;
        }

        /// <summary>
        /// Determines whether <see cref="BeatmapPath"/> should be relative to the stable installation songs folder or not.
        /// </summary>
        public bool UseStablePath => config.UseStablePath;

        /// <summary>
        /// Invoked when a file has been changed in either the project directory or the beatmap directory.
        /// </summary>
        public event Action<ProjectFileType> FileChanged
        {
            add => fileWatcher.FileChanged += value;
            remove => fileWatcher.FileChanged -= value;
        }

        /// <summary>
        /// Gets the beatmap set for this project.
        /// </summary>
        public BeatmapSetInfo BeatmapSet => beatmapManager.BeatmapSet;

        /// <summary>
        /// Gets the beatmap resources for this project.
        /// </summary>
        public IResourceStore<byte[]> Resources => beatmapManager.Resources;

        private bool exists => File.Exists(msBuildProjectPath);
        private string msBuildProjectPath => System.IO.Path.Combine(Path, System.IO.Path.ChangeExtension(Name, "csproj"));
        private string projectConfigPath => System.IO.Path.Combine(Path, System.IO.Path.ChangeExtension(Name, ".sbtw.json"));
        private string beatmapPath => config.BeatmapPath;
        private readonly ProjectConfiguration config;
        private readonly ProjectFileWatcher fileWatcher;
        private readonly ProjectBeatmapManager beatmapManager;

        public Project(ProjectConfiguration config, GameHost host, AudioManager audio, RulesetStore rulesets)
        {
            this.config = config;
            fileWatcher = new ProjectFileWatcher(this);
            beatmapManager = new ProjectBeatmapManager(this, host, audio, rulesets);
        }

        /// <summary>
        /// Builds this project.
        /// </summary>
        public void Build()
        {
            if (exists)
                ProjectHelper.Build(Path);
        }

        /// <summary>
        /// Cleans this project.
        /// </summary>
        public void Clean()
        {
            if (exists)
                ProjectHelper.Clean(Path);
        }

        /// <summary>
        /// Restores this project's dependencies.
        /// </summary>
        public void Restore()
        {
            if (exists)
                ProjectHelper.Restore(Path);
        }

        /// <summary>
        /// Saves this project.
        /// </summary>
        public void Save(bool generateMSBuildProject = false)
        {
            using var stream = File.OpenWrite(projectConfigPath);
            using var writer = new StreamWriter(stream);
            writer.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
            writer.Close();

            if (!generateMSBuildProject)
                return;

            File.WriteAllText(msBuildProjectPath, string.Format(template, resolve_osu_game_version()));
        }

        /// <summary>
        /// Gets the working beatmap for this project.
        /// </summary>
        public WorkingBeatmap GetWorkingBeatmap(string version)
            => beatmapManager.GetWorkingBeatmap(version);


        /// <summary>
        /// Load from an sbtw JSON file. Returns null if the provided project file is invalid.
        /// </summary>
        public static Project Load(string path, GameHost host, AudioManager audioManager, RulesetStore rulesets)
        {
            if (string.IsNullOrEmpty(path) || !path.Contains(".sbtw.json"))
                return null;

            try
            {
                using var stream = File.OpenRead(path);
                using var reader = new StreamReader(stream);
                var config = JsonConvert.DeserializeObject<ProjectConfiguration>(reader.ReadToEnd());
                config.Path = System.IO.Path.GetDirectoryName(path);
                config.Name = System.IO.Path.GetFileNameWithoutExtension(path);

                if (string.IsNullOrEmpty(config.BeatmapPath))
                    return null;

                return new Project(config, host, audioManager, rulesets);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            fileWatcher.Dispose();
            GC.SuppressFinalize(this);
        }

        private static string resolve_osu_game_version()
        {
            var version = typeof(osu.Game.OsuGame).Assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Revision}";
        }

        private static readonly string template = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <None Include=""Beatmap\**\*"" CopyToOutputDirectory=""Never""/>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include=""ppy.osu.Game"" Version=""{0}"" PrivateAssets=""All"" />
  </ItemGroup>

</Project>";
    }
}
