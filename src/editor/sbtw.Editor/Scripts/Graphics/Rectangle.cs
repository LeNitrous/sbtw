// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace sbtw.Editor.Scripts.Graphics
{
    public class Rectangle : Asset
    {
        protected override string Extension => ".png";

        private readonly string name;

        public Rectangle(string name)
        {
            this.name = name;
        }

        protected override string CreateIdentifier() => name;

        protected override void Generate(string path)
        {
            var image = new Image<Rgba32>(1, 1, new Rgba32(255, 255, 255));
            image.SaveAsPng(path);
        }
    }
}
