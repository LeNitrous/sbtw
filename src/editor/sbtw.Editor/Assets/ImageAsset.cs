// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace sbtw.Editor.Assets
{
    public abstract class ImageAsset : Asset
    {
        protected sealed override ReadOnlySpan<byte> Generate()
        {
            var image = GetImage();
            using var memory = new MemoryStream();
            image.Save(memory, PngFormat.Instance);
            return new Span<byte>(memory.ToArray());
        }

        protected abstract Image<Rgba32> GetImage();
    }
}
