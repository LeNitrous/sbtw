// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.IO.Stores;
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
        private readonly IResourceStore<byte[]> resources;

        public ProjectManager(GameHost host, AudioManager audio, RulesetStore rulesets, IResourceStore<byte[]> resources, WorkingBeatmap dummyWorkingBeatmap)
        {
            this.host = host;
            this.audio = audio;
            this.rulesets = rulesets;
            this.resources = resources;
            DefaultProject = new DummyProject(dummyWorkingBeatmap);
        }

        public IProject Load(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.Contains(".sbtw.json"))
                throw new InvalidProjectPathException(@"Provided path is not a project file.");

            using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);

            ProjectTemplateType type = ProjectTemplateType.Freeform;
            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(path)))
            {
                if (Path.GetExtension(file) == ".csproj")
                {
                    type = ProjectTemplateType.CSharp;
                    break;
                }

                if (Path.GetExtension(file) == ".vbproj")
                {
                    type = ProjectTemplateType.VisualBasic;
                    break;
                }
            }

            var project = new Project(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), host, audio, rulesets, resources, type);
            JsonConvert.PopulateObject(reader.ReadToEnd(), project);
            project.Initialize();

            if (string.IsNullOrEmpty(project.BeatmapPath))
                throw new InvalidBeatmapPathException(@"Beamtap path is malformed.");

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
                    throw new PlatformNotSupportedException(@"No stable installation found. Cannot import beatmap difficulty file.");

                if (!beatmapPath.Contains(StableHelper.STABLE_PATH))
                    throw new InvalidOperationException(@"Beatmap file must originate from an existing stable installation.");
            }

            if (!is_path_valid(path))
                throw new InvalidProjectPathException(@"Provided path is not a valid file system path.");

            if (Directory.GetFiles(path).Any())
                throw new NonEmptyProjectPathException(@"Project path is not empty.");

            if (type != ProjectTemplateType.Freeform && !NetDriverHelper.HAS_DOTNET)
                throw new PlatformNotSupportedException(@"Cannot create an MSBuild project when no NET Driver is present. Try making a freeform project instead.");

            Directory.CreateDirectory(path);

            bool isFromArchive = false;
            if (Path.GetExtension(beatmapPath) == ".osz")
            {
                isFromArchive = true;
                ZipFile.ExtractToDirectory(beatmapPath, Path.Combine(path, "Beatmap"));
            }

            var project = new Project(name, path, host, audio, rulesets, resources, type)
            {
                BeatmapPath = isFromArchive ? Path.Combine(path, "Beatmap") : new DirectoryInfo(beatmapPath).Parent.Name,
                UseStablePath = !isFromArchive,
            };

            project.Initialize(true);

            return project;
        }
    }

    public enum ProjectTemplateType
    {
        [Description("C#")]
        CSharp,

        [Description("Visual Basic")]
        VisualBasic,

        Freeform
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
