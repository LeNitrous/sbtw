// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.IO.Stores;

namespace sbtw.Game.Projects.Generators
{
    public class VisualBasicProjectGenerator : ProjectGenerator, IMsBuildProjectGenerator
    {
        public VisualBasicProjectGenerator(IProject project, IResourceStore<byte[]> resources)
            : base(project, resources)
        {
        }

        public override void Generate()
        {
            base.Generate();
            CopyResourceToProject("msbuild.template", $"{Project.Name}.vbproj");
            GenerateScript("MyScript");
        }

        public void GenerateScript(string name)
        {
            WriteTemplateToProject("script-vb.template", $"{name}.vb", Project.Name, name);
        }
    }
}
