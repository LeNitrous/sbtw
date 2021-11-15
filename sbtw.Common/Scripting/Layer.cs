// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public enum Layer
    {
        /// <summary>
        /// Objects placed here are shown at the backmost part of the storyboard.
        /// </summary>
        Background,

        /// <summary>
        /// Objects placed here are shown above <see cref="Background"/>.
        /// </summary>
        Foreground,

        /// <summary>
        /// Objects placed here are only shown when the player is passing.
        /// </summary>
        Passing,

        /// <summary>
        /// Objects placed here are only shown when the player is failing.
        /// </summary>
        Failing,

        /// <summary>
        /// Objects placed here are shown above the playfield.
        /// </summary>
        Overlay,
    }
}
