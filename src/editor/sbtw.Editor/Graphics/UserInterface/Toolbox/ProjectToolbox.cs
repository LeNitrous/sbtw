// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public class ProjectToolbox : EditorTabbedToolbox<string>
    {
        public ProjectToolbox()
            : base("Project")
        {
            AddTab(@"Scripts", new ScriptsTab());
            AddTab(@"Settings", new SettingsTab());
        }
    }
}
