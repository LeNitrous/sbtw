// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace sbtw.Editor.Projects
{
    public class ProjectManager
    {
        private readonly GameHost host;

        public readonly IProject DefaultProject;

        public ProjectManager(GameHost host, IProject defaultProject)
        {
            this.host = host;
            DefaultProject = defaultProject;
        }

        public IProject Load(string path)
        {
            try
            {
                var file = new FileInfo(path);

                if (file.Extension != ".sbtw.json")
                    throw new ArgumentException("File is not a project.");

                using var stream = File.OpenRead(file.FullName);
                using var reader = new StreamReader(stream);

                var project = new Project(Path.GetFileNameWithoutExtension(file.Name), file.FullName, host);
                JsonConvert.PopulateObject(reader.ReadToEnd(), project);

                return project;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load project");
                return null;
            }
        }

        public IProject Create(string name, string path, string beatmapPath = null)
        {
            try
            {
                var projectPath = new DirectoryInfo(path);
                projectPath.Create();

                if (projectPath.GetFiles().Length > 0)
                    throw new ArgumentException("Project directory is not empty.");

                if (!string.IsNullOrEmpty(beatmapPath))
                {
                    var beatmapFile = new FileInfo(beatmapPath);

                    if (beatmapFile.Extension != ".osz")
                        throw new ArgumentException("Beatmap is not a beatmap archive.");

                    ZipFile.ExtractToDirectory(beatmapFile.FullName, projectPath.CreateSubdirectory("beatmap").FullName);
                }

                var project = new Project(name, projectPath.FullName, host);
                project.Save();

                return project;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to create project");
                return null;
            }
        }
    }
}
