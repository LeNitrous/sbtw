// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using NuGet.Packaging;

namespace sbtw.Game.Projects
{
    public class ProjectFileWatcher : IDisposable
    {
        public event Action<ProjectFileType> FileChanged;

        private readonly Project project;
        private readonly FileSystemWatcher projectWatcher;
        private readonly FileSystemWatcher beatmapWatcher;
        private static readonly string[] beatmap_resources = new[]
        {
            "*.osu",
            "*.osb",
            "*.mp3",
            "*.wav",
            "*.mp4",
            "*.avi",
            "*.png",
            "*.jpg",
            "*.jpeg",
            "skin.ini",
        };

        public ProjectFileWatcher(Project project)
        {
            this.project = project;

            projectWatcher = new FileSystemWatcher { Path = project.Path };
            projectWatcher.Filters.Add("*.cs");

            applyFileWatcherSettings(projectWatcher);

            if (!project.UseStablePath)
            {
                projectWatcher.Filters.AddRange(beatmap_resources);
                projectWatcher.IncludeSubdirectories = true;
            }
            else
            {
                beatmapWatcher = new FileSystemWatcher { Path = project.BeatmapPath };
                beatmapWatcher.Filters.AddRange(beatmap_resources);
                beatmapWatcher.IncludeSubdirectories = true;

                applyFileWatcherSettings(beatmapWatcher);
            }
        }

        private void handleFileEvent(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".osu":
                    if (!path.Contains(project.BeatmapPath))
                        return;

                    FileChanged?.Invoke(ProjectFileType.Beatmap);
                    break;

                case ".osb":
                    if (!path.Contains(project.BeatmapPath))
                        return;

                    FileChanged?.Invoke(ProjectFileType.Storyboard);
                    break;

                case ".cs":
                    FileChanged?.Invoke(ProjectFileType.Script);
                    break;

                case "":
                    break;

                default:
                    if (!path.Contains(project.BeatmapPath))
                        return;

                    FileChanged?.Invoke(ProjectFileType.Resource);
                    break;

            }
        }

        private void applyFileWatcherSettings(FileSystemWatcher watcher)
        {
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += (_, e) => handleFileEvent(e.FullPath);
            watcher.Created += (_, e) => handleFileEvent(e.FullPath);
            watcher.Deleted += (_, e) => handleFileEvent(e.FullPath);
            watcher.Renamed += (_, e) => handleFileEvent(e.FullPath);

            watcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            projectWatcher?.Dispose();
            beatmapWatcher?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public enum ProjectFileType
    {
        None,

        /// <summary>
        /// A beatmap file (.osu)
        /// </summary>
        Beatmap,

        /// <summary>
        /// A storyboard file (.osb)
        /// </summary>
        Storyboard,

        /// <summary>
        /// A resource file (images, videos, audio)
        /// </summary>
        Resource,

        /// <summary>
        /// A script file (.cs)
        /// </summary>
        Script
    }
}
