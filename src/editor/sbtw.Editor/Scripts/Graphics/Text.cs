// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace sbtw.Editor.Scripts.Graphics
{
    public class Text : Asset
    {
        private readonly FontConfiguration config;
        private readonly string text;

        public Text(string text, FontConfiguration config)
        {
            this.text = text;
            this.config = config;
        }

        protected override string CreateIdentifier() => $"{text}-{config.Path}-{config.Name}-{config.Size}";

        protected override void Generate(string path)
        {
            string fontFullPath = Script.Storage.GetFullPath(config.Path);

            if (!File.Exists(fontFullPath))
            {
                Script.Error($"Failed to find font in {fontFullPath}");
                return;
            }

            var collection = new FontCollection();
            collection.Install(fontFullPath);

            if (collection.TryFind(config.Name, out var family))
            {
                var font = family.CreateFont(config.Size);
                var size = TextMeasurer.Measure(text, new RendererOptions(font));
                var image = new Image<Rgba32>((int)size.Width, (int)size.Height, new Rgba32(255, 255, 255, 0));
                image.Mutate(ctx => ctx.DrawText(text, font, Color.White, size.Location));
                image.SaveAsPng(path);
            }
            else
            {
                Script.Error($"Failed to find font family {config.Name} from {config.Path}.");
            }
        }
    }

    public struct FontConfiguration
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }

        public FontConfiguration(string path, string name, int size)
        {
            Path = path;
            Name = name;
            Size = size;
        }
    }
}
