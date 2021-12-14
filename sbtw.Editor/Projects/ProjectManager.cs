// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using sbtw.Editor.Resources;

namespace sbtw.Editor.Projects
{
    public class ProjectManager
    {
        private readonly GameHost host;

        public readonly IProject DefaultProject;

        private readonly IResourceStore<byte[]> resources;

        public ProjectManager(GameHost host)
        {
            this.host = host;
            resources = new NamespacedResourceStore<byte[]>(new DllResourceStore(EditorResources.ResourceAssembly), "Resources/Templates");
        }

        public ProjectManager(GameHost host, IProject defaultProject)
            : this(host)
        {
            DefaultProject = defaultProject;
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

        public IProject Create(string name, string path)
        {
            try
            {
                var projectPath = new DirectoryInfo(path);
                projectPath.Create();

                if (projectPath.GetFiles().Length > 0)
                    throw new ArgumentException("Project directory is not empty.");

                var project = new Project(host.GetStorage(path), name);

                copy(project, "sbtw.d.ts");
                copy(project, "jsconfig.json");
                copy(project, "launch.json", "./.vscode/launch.json");
                project.Save();

                return project;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to create project");
                return null;
            }
        }

        private void copy(Project project, string resourceName, string targetDestination = null)
        {
            using var rStream = resources.GetStream(resourceName);
            using var wStream = project.Storage.GetStream(targetDestination ?? resourceName, FileAccess.Write);
            rStream.CopyTo(wStream);
        }
    }
}
