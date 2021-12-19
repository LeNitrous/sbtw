// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace sbtw.Editor.Projects
{
    public class ProjectManager
    {
        private readonly GameHost host;

        public ProjectManager(GameHost host)
        {
            this.host = host;
        }

        public IProject Load(string path)
        {
            try
            {
                var file = new FileInfo(path);

                if (!file.FullName.Contains(".sbtw.json"))
                    throw new ArgumentException("File is not a project.");

                using var stream = File.OpenRead(file.FullName);
                using var reader = new StreamReader(stream);

                var project = new Project(host.GetStorage(path), Path.GetFileNameWithoutExtension(file.Name));
                JsonConvert.PopulateObject(reader.ReadToEnd(), project);

                return project;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load project");
                return null;
            }
        }

        public IProject Create(string name, string path, IEnumerable<IProjectGenerator> generators = null)
        {
            try
            {
                var projectPath = new DirectoryInfo(path);
                projectPath.Create();

                if (projectPath.GetFiles().Length > 0)
                    throw new ArgumentException("Project directory is not empty.");

                var project = new Project(host.GetStorage(path), name);

                if (generators != null)
                {
                    foreach (var generator in generators)
                        generator.Generate(project.Storage);
                }

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
