// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Packaging;

namespace sbtw.Game.Projects
{
    public class ProjectFileWatcher : IDisposable
    {
        public event Action<ProjectFileType> FileChanged;

        private readonly Project project;
        private readonly FileSystemWatcher projectWatcher;
        private readonly FileSystemWatcher beatmapWatcher;
        private readonly List<ObservedFile> observed = new List<ObservedFile>();

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
            projectWatcher.Filters.Add("*.vb");
            projectWatcher.Filters.Add("*.js");
            projectWatcher.Filters.Add("*.py");

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

        private void handleFileEvent(FileSystemEventArgs e)
        {
            // Check if we are already tracking the file. If not, then add it to our list.
            var file = observed.FirstOrDefault(f => f.Path == e.FullPath);
            bool isNewFile = false;

            if (file == null)
            {
                isNewFile = true;
                observed.Add(file = new ObservedFile
                {
                    Path = e.FullPath,
                    LastWriteTime = File.GetLastWriteTime(e.FullPath),
                });
            }

            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                // Remove tracking if the file has been deleted.
                observed.Remove(file);
            }
            else if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                if (e is not RenamedEventArgs renamedArgs)
                    return;

                // Remove the old path if the file has been renamed. We have added the new path beforehand.
                observed.Remove(observed.FirstOrDefault(f => f.Path == renamedArgs.OldFullPath));
            }
            else
            {
                // Check if the last write time is the same. If so, don't invoke events.
                if (DateTime.Compare(file.LastWriteTime, File.GetLastWriteTime(e.FullPath)) == 0 && !isNewFile)
                    return;

                file.LastWriteTime = File.GetLastWriteTime(file.Path);
            }

            switch (Path.GetExtension(e.FullPath))
            {
                case ".osu":
                    if (!e.FullPath.Contains(project.BeatmapPath))
                        return;

                    FileChanged?.Invoke(ProjectFileType.Beatmap);
                    break;

                case ".osb":
                    if (!e.FullPath.Contains(project.BeatmapPath))
                        return;

                    FileChanged?.Invoke(ProjectFileType.Storyboard);
                    break;

                case ".cs":
                case ".vb":
                case ".js":
                case ".py":
                    FileChanged?.Invoke(ProjectFileType.Script);
                    break;

                case "":
                    break;

                default:
                    if (!e.FullPath.Contains(project.BeatmapPath))
                        return;

                    FileChanged?.Invoke(ProjectFileType.Resource);
                    break;

            }
        }

        private void applyFileWatcherSettings(FileSystemWatcher watcher)
        {
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += (_, e) => handleFileEvent(e);
            watcher.Created += (_, e) => handleFileEvent(e);
            watcher.Deleted += (_, e) => handleFileEvent(e);
            watcher.Renamed += (_, e) => handleFileEvent(e);

            watcher.EnableRaisingEvents = true;
        }

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    projectWatcher?.Dispose();
                    beatmapWatcher?.Dispose();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private class ObservedFile : IEquatable<ObservedFile>
        {
            public string Path { get; set; }

            public DateTime LastWriteTime { get; set; }

            public bool Equals(ObservedFile other)
                => other.Path == Path && other.LastWriteTime == LastWriteTime;

            public override bool Equals(object obj)
            {
                if (obj is not ObservedFile otherObservedFile)
                    return false;

                return Equals(otherObservedFile);
            }

            public override int GetHashCode()
                => HashCode.Combine(Path, LastWriteTime);
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
        /// A script file
        /// </summary>
        Script
    }
}
