// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;
using sbtw.Editor.Generators;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Generators
{
    public class OsbGeneratorTest : GeneratorTest<OsbGenerator, StringBuilder>
    {
        protected override OsbGenerator CreateRunner() => new OsbGenerator();

        [Test]
        public void TestHeaderEncoding()
        {
            string[] expected = new[]
            {
                "[Events]",
                "// Background and Video Events",
                "// Storyboard Layer 0 (Background)",
                "// Storyboard Layer 1 (Failing)",
                "// Storyboard Layer 2 (Passing)",
                "// Storyboard Layer 3 (Foreground)",
                "// Storyboard Layer 4 (Overlay)",
                "// Storyboard Sound Samples",
            };
            Assert.That(string.Join(Environment.NewLine, generate()), Is.EqualTo(string.Join(Environment.NewLine, expected)));
        }

        [Test]
        public void TestSpriteEncoding()
        {
            var generated = generate(script =>
            {
                var sprite = script.GetGroup("Test").CreateSprite("test.png");
                sprite.Move(0, 100, new Vector2(100), new Vector2(100));
                sprite.MoveX(100, 200, 0, 320);
                sprite.MoveY(200, 300, 0, 320);
                sprite.Fade(300, 400, 0, 1);
                sprite.Color(400, 500, Colour4.Black, Colour4.White);
                sprite.Color(500, 600, "000", "000");
                sprite.ColorRGB(600, 700, 255, 0, 0, 255, 0, 0);
                sprite.ColorHSL(700, 800, 0, 1, 0.5f, 0, 1, 0.5f);
                sprite.ColorHSV(800, 900, 0, 1, 1, 0, 1, 1);
                sprite.FlipH(900, 1000);
                sprite.FlipV(1000, 1100);
                sprite.Additive(1100, 1200);
                sprite.Scale(1200, 1300, 1, 1.5f);
                sprite.ScaleVec(1300, 1400, Vector2.One, new Vector2(1.5f));
                sprite.StartLoopGroup(1400, 5);
                sprite.Fade(0, 100, 0, 1);
                sprite.EndGroup();
                sprite.StartTriggerGroup("HitSoundClap", 2000, 2100, 0);
                sprite.Fade(0, 100, 0, 1);
                sprite.EndGroup();
            });

            string[] expected = new[]
            {
                "Sprite,Background,TopLeft,\"test.png\",0,0",
                " M,0,0,100,100,100",
                " MX,0,100,200,0,320",
                " MY,0,200,300,0,320",
                " F,0,300,400,0,1",
                " C,0,400,500,0,0,0,255,255,255",
                " C,0,500,600,0,0,0",
                " C,0,600,700,255,0,0",
                " C,0,700,800,255,0,0",
                " C,0,800,900,255,0,0",
                " P,0,900,1000,H",
                " P,0,1000,1100,V",
                " P,0,1100,1200,A",
                " S,0,1200,1300,1,1.5",
                " V,0,1300,1400,1,1,1.5,1.5",
                " L,1400,5",
                "  F,0,0,100,0,1",
                " T,HitSoundClap,2000,2100",
                "  F,0,0,100,0,1"
            };

            Assert.That(string.Join(Environment.NewLine, generated.Skip(3).Take(expected.Length)), Is.EqualTo(string.Join(Environment.NewLine, expected)));
        }

        [Test]
        public void TestAnimationEncoding()
        {
            string generated = generate(s => s.GetGroup("Test").CreateAnimation("test.png")).Skip(3).First();
            Assert.That(generated, Is.EqualTo(@"Animation,Background,TopLeft,""test.png"",0,0,0,0,LoopOnce"));
        }

        [Test]
        public void TestSampleEncoding()
        {
            string generated = generate(s => s.GetGroup("Test").CreateSample("test.wav", 0)).Skip(8).First();
            Assert.That(generated, Is.EqualTo(@"Sample,0,0,""test.wav"",100"));
        }

        [Test]
        public void TestVideoEncoding()
        {
            string generated = generate(s => s.SetVideo("test.mp4", 0)).Skip(2).First();
            Assert.That(generated, Is.EqualTo(@"Video,0,""test.mp4"""));
        }

        private IEnumerable<string> generate(Action<Script> perform = null) => Generate(perform).Result.ToString().TrimEnd().Split(Environment.NewLine);
    }
}
