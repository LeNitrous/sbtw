// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Reflection;
using osu.Framework.IO.Stores;

namespace sbtw.Game.Projects.Generators
{
    public abstract class MsBuildProjectGenerator : ProjectGenerator, IMsBuildProjectGenerator
    {
        protected abstract string Extension { get; }

        protected MsBuildProjectGenerator(IProject project, IResourceStore<byte[]> resources)
            : base(project, resources)
        {
        }

        public override void Generate()
        {
            base.Generate();
            WriteTemplateToProject("msbuild.template", $"{Project.Name}.{Extension}", get_version());
            GenerateScript("MyScript");
        }

        public abstract void GenerateScript(string name);

        private static string get_version()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Revision}";
        }
    }
}
