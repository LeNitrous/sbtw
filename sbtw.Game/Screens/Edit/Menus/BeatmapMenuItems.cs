// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.Linq;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Edit.Components.Menus;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class BeatmapMenuItems : MenuItem
    {
        private readonly GameHost host;
        private readonly IProject project;
        private readonly WorkingBeatmap beatmap;
        private readonly string storyboardPath;
        private readonly EditorMenuItem openStoryboardItem;

        public BeatmapMenuItems(GameHost host, WorkingBeatmap beatmap, IProject project, Action<BeatmapInfo> difficultyChange)
            : base("Beatmap")
        {
            Items = new[]
            {
                new EditorMenuItem("Select Difficulty", MenuItemType.Standard)
                {
                    Items = beatmap?.BeatmapInfo.BeatmapSet.Beatmaps?
                        .Select(b => new DifficultyMenuItem(b, b.Version == beatmap.BeatmapInfo.Version, difficultyChange))
                        .ToArray() ?? new MenuItem[] { new EditorMenuItemSpacer() }
                },
                new EditorMenuItemSpacer(),
                new EditorMenuItem("Reload Beatmap", MenuItemType.Standard, () => difficultyChange?.Invoke(beatmap.BeatmapInfo)),
                new EditorMenuItemSpacer(),
                new EditorMenuItem("Open Beatmap Folder", MenuItemType.Standard, () => host.OpenFileExternally(project.BeatmapPath)),
                new EditorMenuItem("Open Beatmap File", MenuItemType.Standard, openDifficultyFile),
                openStoryboardItem = new EditorMenuItem("Open Storyboard File", MenuItemType.Standard, () => host.OpenFileExternally(storyboardPath)),
            };

            this.host = host;
            this.beatmap = beatmap;
            this.project = project;

            foreach (var item in Items.Skip(1))
                item.Action.Disabled = beatmap is DummyWorkingBeatmap;

            if (project is DummyProject)
                return;

            storyboardPath = Directory.GetFiles(project.BeatmapPath).FirstOrDefault(f => Path.GetExtension(f) == ".osb");

            if (string.IsNullOrEmpty(storyboardPath))
                openStoryboardItem.Action.Disabled = string.IsNullOrEmpty(storyboardPath);
        }

        private void openDifficultyFile()
        {
            string path = Directory.GetFiles(project.BeatmapPath)
                .FirstOrDefault(f => f.Contains($"[{beatmap.BeatmapInfo.Version}]"));

            if (!string.IsNullOrEmpty(path))
                host.OpenFileExternally(path);
        }
    }
}
