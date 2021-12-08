// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Platform;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Edit.Components.Menus;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class ProjectMenuItems : MenuItem
    {
        private readonly GameHost host;
        private readonly EditorMenuItem openInExplorer;
        private readonly EditorMenuItem openInCode;
        private readonly IProject project;

        public ProjectMenuItems(GameHost host, IProject project, Action generateStoryboard)
            : base("Project")
        {
            Items = new[]
            {
                new EditorMenuItem("Run", MenuItemType.Highlighted, generateStoryboard),
                new EditorMenuItemSpacer(),
                new EditorMenuItem("Build", MenuItemType.Standard, () => project.Build()),
                new EditorMenuItem("Clean", MenuItemType.Standard, () => project.Clean()),
                new EditorMenuItem("Restore", MenuItemType.Standard, () => project.Restore()),
                new EditorMenuItemSpacer(),
                openInExplorer = new EditorMenuItem("Open in File Explorer", MenuItemType.Standard, presentProjectFolder),
                openInCode = new EditorMenuItem("Open in Code", MenuItemType.Standard, presentProjectFolderInCode),
            };

            this.project = project;
            this.host = host;

            foreach (var item in Items.Take(5))
                item.Action.Disabled = !ProjectHelper.HAS_DOTNET || project is DummyProject;

            openInCode.Action.Disabled = !ProjectHelper.EDITORS.Any() || project is DummyProject;
            openInExplorer.Action.Disabled = project is DummyProject;
        }

        private void presentProjectFolder() => host.OpenFileExternally(project.Path);

        private void presentProjectFolderInCode() => Process.Start(new ProcessStartInfo
        {
            FileName = ProjectHelper.EDITORS.FirstOrDefault().Key,
            Arguments = project.Path,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true,
        });

    }
}
