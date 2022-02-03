// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace sbtw.Editor.Assets
{
    public class Rectangle : Asset
    {
        protected override void Generate(string path)
        {
            var image = new Image<Rgba32>(1, 1, new Rgba32(255, 255, 255));
            image.SaveAsPng(System.IO.Path.ChangeExtension(path, ".png"));
        }
    }
}
