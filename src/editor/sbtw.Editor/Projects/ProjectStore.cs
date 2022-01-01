// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Audio;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Rulesets;

namespace sbtw.Editor.Projects
{
    public class ProjectStore
    {
        private readonly GameHost host;
        private readonly AudioManager audio;
        private readonly RulesetStore rulesets;

        public ProjectStore(GameHost host, AudioManager audio, RulesetStore rulesets)
        {
            this.host = host;
            this.audio = audio;
            this.rulesets = rulesets;
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

                var project = new Project(host, audio, rulesets, host.GetStorage(Path.GetDirectoryName(path)), Path.GetFileNameWithoutExtension(file.Name));
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

                var project = new Project(host, audio, rulesets, host.GetStorage(path), name);

                if (generators != null)
                {
                    foreach (var generator in generators)
                        generator?.Generate(project.Files);
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
