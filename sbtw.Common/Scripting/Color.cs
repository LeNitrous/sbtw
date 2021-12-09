// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Graphics;

namespace sbtw.Common.Scripting
{
    public struct Color
    {
        /// <summary>
        /// Gets or sets the red color channel value.
        /// </summary>
        public float R
        {
            get => r;
            set => r = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets or sets the green color channel value.
        /// </summary>
        public float G
        {
            get => g;
            set => g = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets or sets the blue color channel value.
        /// </summary>
        public float B
        {
            get => b;
            set => b = Math.Clamp(value, 0, 1);
        }

        private float r;
        private float g;
        private float b;

        public Color(float r, float g, float b)
        {
            this.r = 0.0f;
            this.g = 0.0f;
            this.b = 0.0f;
            R = r;
            G = g;
            B = b;
        }

        public static implicit operator Colour4(Color c) => new Colour4(c.R, c.G, c.B, 1.0f);

        public static implicit operator Color(Colour4 c) => new Color(c.R, c.G, c.B);
    }
}
