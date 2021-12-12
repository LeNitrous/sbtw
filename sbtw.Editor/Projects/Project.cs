// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;

namespace sbtw.Editor.Projects
{
    public class Project : IProject
    {
        public string Name { get; }

        public string Path { get; }

        public readonly Storage Storage;

        private readonly GameHost host;

        public Project(string name, string path, GameHost host)
        {
            Name = name;
            Path = path;
            Storage = (host as DesktopGameHost)?.GetStorage(path);
        }
    }
}
