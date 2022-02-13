// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlobExpressions;
using osu.Framework.Graphics;
using osu.Framework.Threading;
using osu.Game.Extensions;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    public class FileWatcher : Component
    {
        private readonly IFileBackedProject project;
        private readonly FileSystemWatcher watcher;
        private readonly IEnumerable<string> filters;
        private readonly IEnumerable<string> exclude;

        public FileWatcher(IFileBackedProject project)
        {
            this.project = project;

            watcher = new FileSystemWatcher
            {
                Path = project.Files.GetFullPath("."),
                IncludeSubdirectories = true
            };

            var languages = project.Scripts.Languages.OfType<FileBasedScriptLanguage>();
            filters = languages.SelectMany(lang => lang.Extensions).Select(ext => $"*.{ext}");
            exclude = languages.SelectMany(lang => lang.Exclude);

            watcher.Filters.AddRange(filters.Concat(extensions));
            watcher.NotifyFilter = NotifyFilters.LastWrite
                | NotifyFilters.CreationTime
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;

            watcher.Created += (_, e) => handleChange(e.FullPath);
            watcher.Deleted += (_, e) => handleChange(e.FullPath);
            watcher.Renamed += (_, e) => handleChange(e.FullPath);
            watcher.Changed += (_, e) => handleChange(e.FullPath);
        }

        private ScheduledDelegate debounce;

        private void handleChange(string fullPath)
        {
            if (exclude.Any(ex => Glob.IsMatch(fullPath, ex)))
                return;

            debounce?.Cancel();
            debounce = Scheduler.AddDelayed(() =>
            {
                string path = fullPath.Replace(project.Files.GetFullPath("."), string.Empty);

            }, 500);
        }

        private static readonly string[] extensions = new[]
        {
            ".osu",
            "skin.ini",

            ".png",
            ".jpg",
            ".jpeg",

            ".mp3",
            ".wav",
            ".ogg",

            ".mp4",
            ".avi",
            ".flv"
        };
    }
}
