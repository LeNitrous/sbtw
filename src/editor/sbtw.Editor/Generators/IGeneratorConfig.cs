// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;

namespace sbtw.Editor.Generators
{
    public interface IGeneratorConfig
    {
        /// <summary>
        /// When true, offsets the top-left corner by 107 pixels to the left.
        /// </summary>
        BindableBool UseWidescreen { get; }

        /// <summary>
        /// The number of decimals to be used for move-related commands.
        /// </summary>
        BindableInt PrecisionMove { get; }

        /// <summary>
        /// The number of decimals to be used for scale-related commands.
        /// </summary>
        BindableInt PrecisionScale { get; }

        /// <summary>
        /// The number of decimals to be used during storyboard exporting for alpha-related commands.
        /// </summary>
        BindableInt PrecisionAlpha { get; }

        /// <summary>
        /// The number of decimals to be used during storyboard exporting for rotation-related commands.
        /// </summary>
        BindableInt PrecisionRotation { get; }
    }
}
