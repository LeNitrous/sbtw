// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Threading;
using sbtw.Editor.Configuration;
using sbtw.Editor.Languages;

namespace sbtw.Editor.Projects
{
    public class ProjectWatcher : Component
    {
        private FileSystemWatcher watcher;
        private Bindable<bool> hotReload;
        private Bindable<IProject> project;

        [Resolved(canBeNull: true)]
        private Editor editor { get; set; }

        [Resolved]
        private LanguageStore languages { get; set; }

        [BackgroundDependencyLoader]
        private void load(Bindable<IProject> project, EditorConfigManager config)
        {
            hotReload = config.GetBindable<bool>(EditorSetting.HotReload);
            hotReload.ValueChanged += _ => restartWatcher();

            this.project = project.GetBoundCopy();
            this.project.ValueChanged += _ => restartWatcher();

            restartWatcher();
        }

        private void restartWatcher()
        {
            clearWatcher();

            if (project.Value is DummyProject || !hotReload.Value)
                return;

            watcher = new FileSystemWatcher
            {
                Path = project.Value.Path,
                IncludeSubdirectories = true
            };

            foreach (string ext in extensions.Concat(languages.Extensions))
            {
                string filter = ext;

                if (!filter.StartsWith("*") && filter.StartsWith("."))
                    filter = "*" + filter;

                watcher.Filters.Add(filter);
            }

            watcher.NotifyFilter = NotifyFilters.LastWrite
                | NotifyFilters.CreationTime
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;

            watcher.Created += (_, e) => handleChange(e.FullPath);
            watcher.Deleted += (_, e) => handleChange(e.FullPath);
            watcher.Renamed += (_, e) => handleChange(e.FullPath);
            watcher.Changed += (_, e) => handleChange(e.FullPath);

            watcher.EnableRaisingEvents = true;
        }

        private ScheduledDelegate debounce;

        private void handleChange(string path)
        {
            debounce?.Cancel();
            debounce = Scheduler.AddDelayed(() =>
            {
                if (languages.Extensions.Contains(Path.GetExtension(path)))
                    editor?.Generate(GenerateKind.Storyboard);

                if (new FileInfo(path).Directory.Parent.Name == "Beatmap" && extensions.Contains(Path.GetExtension(path)))
                    editor?.RefreshBeatmap();
            }, 500);
        }

        private void clearWatcher()
        {
            watcher?.Dispose();
            watcher = null;
        }

        protected override void Dispose(bool isDisposing)
        {
            clearWatcher();
            base.Dispose(isDisposing);
        }

        private static readonly string[] extensions = new[]
        {
            // osu!
            ".osu",
            "skin.ini",

            // Images
            ".png",
            ".jpg",
            ".jpeg",

            // Audio
            ".mp3",
            ".wav",

            // Video
            ".mp4",
            ".flv",
            ".avi",
        };
    }
}
