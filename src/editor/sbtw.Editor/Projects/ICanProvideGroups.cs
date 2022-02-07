// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    /// <summary>
    /// Determines that the implementer can manage groups.
    /// </summary>
    public interface ICanProvideGroups
    {
        GroupCollection Groups { get; }
    }
}
