// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;
using sbtw.Editor.Generators;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Tests.Generators
{
    public class OsbGeneratorTest : GeneratorTestBase<OsbGenerator, Dictionary<string, StringBuilder>, StringBuilder>
    {
        protected override OsbGenerator CreateGenerator(ICanProvideScripts provider) => new OsbGenerator(provider);

        [Test]
        public void TestGenerateSprite()
        {
            var generated = Generate(s =>
            {
                var sprite = s.GetGroup("Test").CreateSprite("test.png");
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
                sprite.Rotate(2100, 2200, 0, 180);
            })["Background"].ToString().TrimEnd().Split(Environment.NewLine);

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
                " R,0,2100,2200,0,180",
                " L,1400,5",
                "  F,0,0,100,0,1",
                " T,HitSoundClap,2000,2100",
                "  F,0,0,100,0,1",
            };

            Assert.That(generated, Is.EqualTo(expected));
        }

        [Test]
        public void TestGenerateAnimation()
        {
            string generated = Generate(s => s.GetGroup("Test").CreateAnimation("test.png"))["Background"].ToString().TrimEnd();
            Assert.That(generated, Is.EqualTo(@"Animation,Background,TopLeft,""test.png"",0,0,0,0,LoopForever"));
        }

        [Test]
        public void TestGenerateVideo()
        {
            string generated = Generate(s => s.GetGroup("Test").CreateVideo("test.mp4", 0))["Video"].ToString().TrimEnd();
            Assert.That(generated, Is.EqualTo(@"Video,0,""test.mp4"""));
        }

        [Test]
        public void TestGenerateSample()
        {
            string generated = Generate(s => s.GetGroup("Test").CreateSample("test.wav", 0))["Samples"].ToString().TrimEnd();
            Assert.That(generated, Is.EqualTo(@"Sample,0,0,""test.wav"",100"));
        }
    }
}
