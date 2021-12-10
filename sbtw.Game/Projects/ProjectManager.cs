// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using sbtw.Game.Utils;

namespace sbtw.Game.Projects
{
    public class ProjectManager
    {
        public static readonly string EXTENSION = ".sbtw.json";

        public readonly IProject DefaultProject;

        private readonly GameHost host;
        private readonly AudioManager audio;
        private readonly RulesetStore rulesets;

        public ProjectManager(GameHost host, AudioManager audio, RulesetStore rulesets, WorkingBeatmap dummyWorkingBeatmap)
        {
            this.host = host;
            this.audio = audio;
            this.rulesets = rulesets;
            DefaultProject = new DummyProject(dummyWorkingBeatmap);
        }

        public IProject Load(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.Contains(".sbtw.json"))
                throw new InvalidProjectPathException(@"Provided path is not a project file.");

            using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);

            var project = new Project(host, audio, rulesets)
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Path = Path.GetDirectoryName(path),
            };

            JsonConvert.PopulateObject(reader.ReadToEnd(), project);

            if (string.IsNullOrEmpty(project.BeatmapPath))
                throw new InvalidBeatmapPathException(@"Beamtap path is malformed.");

            project.Initialize();

            return project;
        }

        public IProject Create(string name, string path, string beatmapPath, ProjectTemplateType type = ProjectTemplateType.CSharp)
        {
            static bool is_path_valid(string p) => !string.IsNullOrEmpty(p) && Path.IsPathFullyQualified(p);

            if (!is_path_valid(beatmapPath) || !new[] { ".osu", ".osz" }.Any(ext => Path.GetExtension(beatmapPath) == ext))
                throw new InvalidBeatmapPathException(@"Provided path is not a valid beatmap.");

            if (!File.Exists(beatmapPath))
                throw new FileNotFoundException();

            if (Path.GetExtension(beatmapPath) == ".osu")
            {
                if (!StableHelper.HAS_STABLE)
                    throw new PlatformNotSupportedException(@"Attempted to import a beatmap file but system does not have stable installed.");

                if (!beatmapPath.Contains(StableHelper.STABLE_PATH))
                    throw new InvalidOperationException(@"Beatmap file must originate from an existing stable installation.");
            }

            if (!is_path_valid(path))
                throw new InvalidProjectPathException(@"Provided path is not a valid file system path.");

            if (Directory.GetFiles(path).Any())
                throw new NonEmptyProjectPathException(@"Project path is not empty.");

            Directory.CreateDirectory(path);

            bool isFromArchive = false;
            if (Path.GetExtension(beatmapPath) == ".osz")
            {
                isFromArchive = true;
                ZipFile.ExtractToDirectory(beatmapPath, Path.Combine(path, "Beatmap"));
            }

            (string projectExtension, string scriptExtension, string scriptTemplate) = Templates.SCRIPT_TEMPLATE_MAP[type];

            File.WriteAllText(Path.Combine(path, Path.ChangeExtension(name, projectExtension)), Templates.PROJECT);
            File.WriteAllText(Path.Combine(path, Path.ChangeExtension("MyScript", scriptExtension)), scriptTemplate);

            var project = new Project(host, audio, rulesets)
            {
                Name = name,
                Path = path,
                BeatmapPath = isFromArchive ? Path.Combine(path, "beatmap") : new DirectoryInfo(beatmapPath).Parent.Name,
                UseStablePath = isFromArchive,
            };

            project.Save();
            project.Initialize();
            project.Build();

            return project;
        }

        private static class Templates
        {
            public static readonly Dictionary<ProjectTemplateType, (string, string, string)> SCRIPT_TEMPLATE_MAP = new Dictionary<ProjectTemplateType, (string, string, string)>
            {
                { ProjectTemplateType.CSharp, (".csproj", ".cs", script_cs) },
                { ProjectTemplateType.VisualBasic, (".vbproj", ".vb", script_vb) }
            };

            public static readonly string PROJECT = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <None Include=""Beatmap\**\*"" CopyToOutputDirectory=""Never""/>
  </ItemGroup>

</Project>";

            private static readonly string script_cs = @"using sbtw.Common.Scripting;

namespace Project
{
    public class MyScript : Script
    {
        public void Generate()
        {
        }
    }
}
";

            private static readonly string script_vb = @"Imports sbtw.Common.Scripting

Namespace Project
    Public Class MyScript
        Inherits Script

        Public Sub Generate()
        End Sub
    End Class
End Namespace
";
        }
    }

    public enum ProjectTemplateType
    {
        [Description("C#")]
        CSharp,

        [Description("Visual Basic")]
        VisualBasic
    }

    public class InvalidBeatmapPathException : Exception
    {
        public InvalidBeatmapPathException(string message)
            : base(message)
        {
        }
    }

    public class InvalidProjectPathException : Exception
    {
        public InvalidProjectPathException(string message)
            : base(message)
        {
        }
    }

    public class NonEmptyProjectPathException : Exception
    {
        public NonEmptyProjectPathException(string message)
            : base(message)
        {
        }
    }
}
