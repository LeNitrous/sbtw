// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Projects
{
    public interface IFileBackedProject : IProject, ICanProvideFiles, ICanProvideAssets, ICanProvideScripts, ICanProvideBeatmap
    {
        /// <summary>
        /// The project's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The local absolute path to this project.
        /// </summary>
        string Path { get; }
    }
}
