// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace sbtw.Editor.Scripts.Assets
{
    public class Rectangle : ImageAsset
    {
        protected override Image<Rgba32> GetImage()
            => new Image<Rgba32>(1, 1, new Rgba32(255, 255, 255));
    }
}
