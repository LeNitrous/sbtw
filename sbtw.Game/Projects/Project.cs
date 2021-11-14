// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Microsoft.Build.Exceptions;
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
        public string Name { get; private set; }

        /// <summary>
        /// The absolute path to the project.
        /// </summary>
        public string Path { get; private set; }

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
            private set => beatmapPath = value;
        }

        /// <summary>
        /// Determines whether <see cref="BeatmapPath"/> should be relative to the stable installation songs folder or not.
        /// </summary>
        public bool UseStablePath { get; private set; }

        /// <summary>
        /// Determines whether this project exists on disk. This is always true for instances created using <see cref="Load"/>
        /// and false for instances created in-memory until <see cref="Save"/> is called.
        /// </summary>
        public bool Exists { get; private set; }

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

        private string beatmapPath;
        private readonly ProjectFileWatcher fileWatcher;
        private readonly ProjectBeatmapManager beatmapManager;

        public Project(ProjectConfiguration config, GameHost host, AudioManager audio, RulesetStore rulesets)
        {
            Name = config.Name;
            Path = config.Path;
            BeatmapPath = config.BeatmapPath;
            UseStablePath = config.UseStablePath;

            fileWatcher = new ProjectFileWatcher(this);
            beatmapManager = new ProjectBeatmapManager(this, host, audio, rulesets);
        }

        /// <summary>
        /// Builds this project.
        /// </summary>
        public void Build()
        {
            if (Exists)
                ProjectHelper.Build(Path);
        }

        /// <summary>
        /// Cleans this project.
        /// </summary>
        public void Clean()
        {
            if (Exists)
                ProjectHelper.Clean(Path);
        }

        /// <summary>
        /// Restores this project's dependencies.
        /// </summary>
        public void Restore()
        {
            if (Exists)
                ProjectHelper.Restore(Path);
        }

        /// <summary>
        /// Saves this project as an MSBuild Project file.
        /// </summary>
        public void Save()
        {
            Exists = true;
            File.WriteAllText(System.IO.Path.Combine(Path, System.IO.Path.ChangeExtension(Name, "csproj")), string.Format(template, beatmapPath, UseStablePath, resolve_osu_game_version()));
        }

        /// <summary>
        /// Gets the working beatmap for this project.
        /// </summary>
        public WorkingBeatmap GetWorkingBeatmap(string version)
            => beatmapManager.GetWorkingBeatmap(version);


        /// <summary>
        /// Load from an MSBuild Project file. Returns null if the provided project file is invalid.
        /// </summary>
        public static Project Load(string path, GameHost host, AudioManager audioManager, RulesetStore rulesets)
        {
            if (string.IsNullOrEmpty(path) || System.IO.Path.GetExtension(path) != ".csproj")
                return null;

            try
            {
                var project = ProjectRootElement.Open(path);

                string beatmapPath = project.GetPropertyValue("sbtwBeatmapPath");

                if (string.IsNullOrEmpty(beatmapPath))
                    return null;

                var config = new ProjectConfiguration
                {
                    Path = System.IO.Path.GetDirectoryName(path),
                    Name = System.IO.Path.GetFileNameWithoutExtension(path),
                    BeatmapPath = beatmapPath,
                    UseStablePath = project.GetPropertyValue("sbtwUseStablePath").ToLowerInvariant() == "true",
                };

                var SBTWProject = new Project(config, host, audioManager, rulesets) { Exists = true };

                return SBTWProject;
            }
            catch (InvalidProjectFileException)
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

  <PropertyGroup>
    <sbtwBeatmapPath>{0}</sbtwBeatmapPath>
    <sbtwUseStablePath>{1}</sbtwUseStablePath>
  </PropertyGroup>

  <ItemGroup>
    <None Include=""Beatmap\**\*"" CopyToOutputDirectory=""Never""/>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include=""ppy.osu.Game"" Version=""{2}"" PrivateAssets=""All"" />
  </ItemGroup>

</Project>";
    }

    internal static class ProjectRootElementExtensions
    {
        public static string GetPropertyValue(this ProjectRootElement root, string propName)
            => root.Properties.FirstOrDefault(p => p.Name == propName)?.Value;
    }
}
