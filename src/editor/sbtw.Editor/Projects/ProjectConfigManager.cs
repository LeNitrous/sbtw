// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Projects
{
    public class ProjectConfigManager : JsonBackedConfigManager
    {
        public override string Filename => Path.ChangeExtension(project.Name, ".sbtw.json");
        private readonly IProject project;

        public ProjectConfigManager(IProject project)
            : base(project.Storage)
        {
            this.project = project;
            Load();
        }
    }
}
