// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Configuration;

namespace sbtw.Editor.Projects
{
    public interface IFileBackedProject : IProject, IConfigManager, ICanProvideFiles, ICanProvideAssets, ICanProvideBeatmap
    {
        /// <summary>
        /// The project's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The absolute path to this project.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The absolute path to the file backing this project.
        /// </summary>
        string FullPath { get; }
    }
}
