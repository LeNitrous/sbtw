// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using osu.Framework.IO.Stores;

namespace sbtw.Game.Projects.Generators
{
    public class ProjectGenerator
    {
        protected readonly IProject Project;
        private readonly IResourceStore<byte[]> resources;

        public ProjectGenerator(IProject project, IResourceStore<byte[]> resources)
        {
            this.Project = project;
            this.resources = resources;
        }

        public virtual void Generate()
        {
            CopyResourceToProject("jsconfig.template", "jsconfig.json");
            CopyResourceToProject("jstyping.template", "sbtw.d.ts");
        }

        protected void CopyResourceToProject(string resource, string destination)
        {
            using var rStream = resources.GetStream(resource);
            using var wStream = Project.Storage.GetStream(destination, FileAccess.Write);
            rStream.CopyTo(wStream);
        }

        protected void WriteTemplateToProject(string resource, string destination, params object[] args)
        {
            using var rStream = resources.GetStream(resource);
            using var wStream = Project.Storage.GetStream(destination, FileAccess.Write);

            using var reader = new StreamReader(rStream);
            using var writer = new StreamWriter(wStream);
            writer.Write(string.Format(reader.ReadToEnd(), args));
        }
    }
}
