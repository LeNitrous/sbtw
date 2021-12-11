// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Bindables;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;

namespace sbtw.Game.Projects
{
    public interface IProject
    {
        /// <summary>
        /// The name of this project.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The absolute path to the project.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The path to the beatmap for this project.
        /// </summary>
        string BeatmapPath { get; }

        /// <summary>
        /// Determines whether <see cref="BeatmapPath"/> should be relative to the stable installation songs folder or not.
        /// </summary>
        bool UseStablePath { get; }

        /// <summary>
        /// The storage for this project.
        /// </summary>
        Storage Storage { get; }

        /// <summary>
        /// Invoked when a file has been changed in either the project directory or the beatmap directory.
        /// </summary>
        event Action<ProjectFileType> FileChanged;

        /// <summary>
        /// Gets the beatmap set for this project.
        /// </summary>
        IBeatmapSetInfo BeatmapSet { get; }

        /// <summary>
        /// Gets the beatmap resources for this project.
        /// </summary>
        IResourceStore<byte[]> Resources { get; }

        /// <summary>
        /// Gets the groups associated with this project.
        /// </summary>
        BindableList<string> Groups { get; }

        /// <summary>
        /// Gets whether the storyboard should be widescreen or not.
        /// </summary>
        Bindable<bool> WidescreenStoryboard { get; }

        /// <summary>
        /// Gets whether the storyboard should show the beatmap background or not.
        /// </summary>
        Bindable<bool> ShowBeatmapBackground { get; }

        /// <summary>
        /// Builds this project.
        /// </summary>
        void Build();

        /// <summary>
        /// Cleans this project.
        /// </summary>
        void Clean();

        /// <summary>
        /// Restores this project's dependencies.
        /// </summary>
        void Restore();

        /// <summary>
        /// Saves this project.
        /// </summary>
        void Save();

        /// <summary>
        /// Gets the working beatmap for this project.
        /// </summary>
        WorkingBeatmap GetWorkingBeatmap(string version);
    }
}
