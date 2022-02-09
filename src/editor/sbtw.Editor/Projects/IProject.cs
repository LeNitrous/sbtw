// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;

namespace sbtw.Editor.Projects
{
    public interface IProject : ICanProvideGroups
    {
        /// <summary>
        /// The number of decimals to be exported during storyboard exporting.
        /// </summary>
        BindableInt Precision { get; }
    }
}
