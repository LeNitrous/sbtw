// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Assets;

namespace sbtw.Editor.Projects
{
    /// <summary>
    /// Determines that the implementer can manage assets.
    /// </summary>
    public interface ICanProvideAssets
    {
        AssetCollection Assets { get; }
    }
}