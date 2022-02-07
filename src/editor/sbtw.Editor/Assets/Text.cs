// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace sbtw.Editor.Assets
{
    public class Text : ImageAsset
    {
        private readonly FontConfiguration config;
        private readonly string text;

        public Text(string text, FontConfiguration config)
        {
            this.text = text;
            this.config = config;
        }

        protected override Image<Rgba32> GetImage()
        {
            string fontFullPath = Storage.GetFullPath(config.Path);

            if (!Storage.Exists(fontFullPath))
                throw new FileNotFoundException($@"Failed to find font in ""{fontFullPath}"".");

            var collection = new FontCollection();
            collection.Install(fontFullPath);

            if (!collection.TryFind(config.Name, out var family))
                throw new ArgumentOutOfRangeException($@"Failed to find font family ""{config.Name}"" from ""{config.Path}"".");

            var font = family.CreateFont(config.Size);
            var size = TextMeasurer.Measure(text, new RendererOptions(font));
            var image = new Image<Rgba32>((int)size.Width, (int)size.Height, new Rgba32(255, 255, 255, 0));
            image.Mutate(ctx => ctx.DrawText(text, font, Color.White, size.Location));

            return image;
        }

        public override bool Equals(Asset other)
            => base.Equals(other)
                && ((other as Text)?.text.Equals(text) ?? false)
                && ((other as Text)?.config.Equals(config) ?? false);
    }

    public struct FontConfiguration
    {
        public readonly string Path;
        public readonly string Name;
        public readonly int Size;

        public FontConfiguration(string path, string name, int size)
        {
            Path = path;
            Name = name;
            Size = size;
        }
    }
}
