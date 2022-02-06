// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using osu.Framework.Audio;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Rulesets;

namespace sbtw.Editor.Projects
{
    public class ProjectManager
    {
        private readonly GameHost host;
        private readonly AudioManager audio;
        private readonly RulesetStore rulesets;

        public ProjectManager(GameHost host, AudioManager audio, RulesetStore rulesets)
        {
            this.host = host;
            this.audio = audio;
            this.rulesets = rulesets;
        }

        public IProject Load(string path)
        {
            try
            {
                return new Project(host, audio, rulesets, path);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load project");
                return null;
            }
        }

        public IProject Create(string path, IEnumerable<IProjectGenerator> generators = null)
        {
            try
            {
                var projectPath = new DirectoryInfo(path);
                projectPath.Create();

                if (projectPath.GetFiles().Length > 0)
                    throw new ArgumentException("Project directory is not empty.");

                var project = new Project(host, audio, rulesets, path);

                if (generators != null)
                {
                    foreach (var generator in generators)
                        generator?.Generate(project.Storage);
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
