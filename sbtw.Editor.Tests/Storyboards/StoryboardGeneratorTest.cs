// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Storyboards;
using osuTK;
using sbtw.Editor.Storyboards;

namespace sbtw.Editor.Tests.Storyboards
{
    public class StoryboardGeneratorTest : ScriptRunnerTest<StoryboardGenerator, Storyboard>
    {
        protected override StoryboardGenerator CreateRunner() => new StoryboardGenerator(new BeatmapInfo());

        [Test]
        public void TestVideoGeneration()
        {
            var generated = Generate(script => script.SetVideo("test", 1000));
            var layer = generated.Layers.FirstOrDefault(l => l.Name == "Video");
            Assert.That(layer.Elements.Count, Is.EqualTo(1));

            var video = layer.Elements.FirstOrDefault() as StoryboardVideo;
            Assert.That(video, Is.Not.Null);
            Assert.That(video.Path, Is.EqualTo("test"));
            Assert.That(video.StartTime, Is.EqualTo(1000));
        }

        [Test]
        public void TestSpriteGeneration()
        {
            var generated = Generate(script =>
            {
                var group = script.GetGroup("Test");
                var spriteA = group.CreateSprite("a", Anchor.Centre, Vector2.One);
                spriteA.Fade(1000, 1.0f);

                var spriteB = group.CreateSprite("b");
            });

            var layer = generated.Layers.FirstOrDefault(l => l.Name == "Background");
            Assert.That(layer.Elements.Count, Is.EqualTo(2));

            var sprite = layer.Elements.FirstOrDefault() as StoryboardSprite;
            Assert.That(sprite, Is.Not.Null);
            Assert.That(sprite.Path, Is.EqualTo("a"));
            Assert.That(sprite.Origin, Is.EqualTo(Anchor.Centre));
            Assert.That(sprite.InitialPosition, Is.EqualTo(Vector2.One));
            Assert.That(sprite.HasCommands, Is.True);
        }

        [Test]
        public void TestSampleGeneration()
        {
            var generated = Generate(script => script.GetGroup("Test").CreateSample("a", 1000, 50));

            var layer = generated.Layers.FirstOrDefault(l => l.Name == "Background");
            Assert.That(layer.Elements.Count, Is.EqualTo(1));

            var sample = layer.Elements.FirstOrDefault() as StoryboardSampleInfo;
            Assert.That(sample, Is.Not.Null);
            Assert.That(sample.Path, Is.EqualTo("a"));
            Assert.That(sample.Volume, Is.EqualTo(50));
            Assert.That(sample.StartTime, Is.EqualTo(1000));
        }
    }
}
