// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;

namespace sbtw.Common.Scripting
{
    public struct Color
    {
        /// <summary>
        /// Gets or sets the red color channel value.
        /// </summary>
        public float R { get; set; }

        /// <summary>
        /// Gets or sets the green color channel value.
        /// </summary>
        public float G { get; set; }

        /// <summary>
        /// Gets or sets the blue color channel value.
        /// </summary>
        public float B { get; set; }

        public Color(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static implicit operator Colour4(Color c) => new Colour4(c.R, c.G, c.B, 1.0f);
    }
}
