// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public interface IScriptedStoryboardElement
    {
        /// <summary>
        /// The <see cref="StoryboardScript"/> that instantiated this element.
        /// </summary>
        StoryboardScript Owner { get; }

        /// <summary>
        /// The <see cref="StoryboardLayerName"/> for this element.
        /// </summary>
        StoryboardLayerName Layer { get; }
    }
}
