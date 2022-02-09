// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using sbtw.Editor.Projects;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace sbtw.Editor.Assets
{
    public class Text : ImageAsset
    {
        internal readonly FontConfiguration Config;
        internal readonly string DisplayText;

        public Text(string text, FontConfiguration config)
        {
            DisplayText = text;
            Config = config;
        }

        protected override Image<Rgba32> GetImage()
        {
            var storage = (Project as ICanProvideFiles).Files;

            string fontFullPath = storage.GetFullPath(Config.Path);

            if (!storage.Exists(fontFullPath))
                throw new FileNotFoundException($@"Failed to find font in ""{fontFullPath}"".");

            var collection = new FontCollection();
            collection.Install(fontFullPath);

            if (!collection.TryFind(Config.Name, out var family))
                throw new ArgumentOutOfRangeException($@"Failed to find font family ""{Config.Name}"" from ""{Config.Path}"".");

            var font = family.CreateFont(Config.Size);
            var size = TextMeasurer.Measure(DisplayText, new RendererOptions(font));
            var image = new Image<Rgba32>((int)size.Width, (int)size.Height, new Rgba32(255, 255, 255, 0));
            image.Mutate(ctx => ctx.DrawText(DisplayText, font, Color.White, size.Location));

            return image;
        }

        public override bool Equals(Asset other)
            => base.Equals(other)
                && ((other as Text)?.DisplayText.Equals(DisplayText) ?? false)
                && ((other as Text)?.Config.Equals(Config) ?? false);
    }

    public struct FontConfiguration : IEquatable<FontConfiguration>
    {
        public readonly string Path;
        public readonly string Name;
        public readonly int Size;

        public FontConfiguration(string path, string name, int size)
        {
            Path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentException(@"Argument must contain a valid value.", nameof(path));
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentException(@"Argument must contain a valid value.", nameof(name));
            Size = size;
        }

        public bool Equals(FontConfiguration other)
            => other.Path.Equals(Path) && other.Name.Equals(Name) && other.Size.Equals(Size);

        public override bool Equals(object obj)
            => obj is FontConfiguration configuration && Equals(configuration);

        public override int GetHashCode()
            => HashCode.Combine(Path, Name, Size);

        public static bool operator ==(FontConfiguration left, FontConfiguration right)
            => left.Equals(right);

        public static bool operator !=(FontConfiguration left, FontConfiguration right)
            => !(left == right);
    }
}
