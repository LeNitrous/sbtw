// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.IO.Stores;

namespace sbtw.Game.Projects.Generators
{
    public class CSharpProjectGenerator : MsBuildProjectGenerator
    {
        protected override string Extension => "csproj";

        public CSharpProjectGenerator(IProject project, IResourceStore<byte[]> resources)
            : base(project, resources)
        {
        }

        public override void GenerateScript(string name)
        {
            WriteTemplateToProject("script-cs.template", $"{name}.cs", Project.Name, name);
        }
    }
}
