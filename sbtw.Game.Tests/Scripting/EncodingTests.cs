// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using osu.Game.Storyboards;
using sbtw.Common.Scripting;
using osuAnchor = osu.Framework.Graphics.Anchor;
using osuVector = osuTK.Vector2;

namespace sbtw.Game.Tests.Scripting
{
    public class EncodingTests
    {
        [Test]
        public void TestSpriteEncoding()
        {
            Assert.That(
                new ScriptedSprite(null, Layer.Background, "test.png", osuAnchor.TopLeft, osuVector.Zero).Encode().TrimEnd(Environment.NewLine.ToCharArray()),
                Is.EqualTo("Sprite,Background,TopLeft,\"test.png\",0,0")
            );

            createSpriteTimelineTest(s => s.Move(Easing.None, 0, 1000, Vector2.Zero, new Vector2(320)), @" MX,0,0,1000,0,320
 MY,0,0,1000,0,320");

            createSpriteTimelineTest(s => s.MoveX(Easing.None, 0, 1000, 0, 320), " MX,0,0,1000,0,320");
            createSpriteTimelineTest(s => s.MoveY(Easing.None, 0, 1000, 0, 320), " MY,0,0,1000,0,320");
            createSpriteTimelineTest(s => s.Fade(Easing.None, 0, 1000, 0, 1), " F,0,0,1000,0,1");
            createSpriteTimelineTest(s => s.Color(Easing.None, 0, 1000, new Color(0, 0, 0), new Color(1, 1, 1)), " C,0,0,1000,0,0,0,1,1,1");
            createSpriteTimelineTest(s => s.FlipH(Easing.None, 0, 1000), " P,0,0,1000,H");
            createSpriteTimelineTest(s => s.FlipV(Easing.None, 0, 1000), " P,0,0,1000,V");
            createSpriteTimelineTest(s => s.Additive(Easing.None, 0, 1000), " P,0,0,1000,A");
            createSpriteTimelineTest(s => s.Scale(Easing.None, 0, 1000, 1, 1.5f), " S,0,0,1000,1,1.5");
            createSpriteTimelineTest(s => s.ScaleVec(Easing.None, 0, 1000, Vector2.One, new Vector2(1.5f)), " V,0,0,1000,1,1,1.5,1.5");

            createSpriteTimelineTest(s =>
            {
                s.StartLoopGroup(0, 5);
                s.Fade(Easing.None, 0, 1000, 0, 1);
                s.EndGroup();
            }, @" L,0,5
  F,0,0,1000,0,1");

            createSpriteTimelineTest(s =>
            {
                s.StartTriggerGroup("HitSoundClap", 0, 1000, 0);
                s.Fade(Easing.None, 0, 1000, 0, 1);
                s.EndGroup();
            }, @" T,HitSoundClap,0,1000
  F,0,0,1000,0,1");
        }

        [Test]
        public void TestAnimationEncoding()
        {
            Assert.That(
                new ScriptedAnimation(null, Layer.Background, "test.png", osuAnchor.TopLeft, osuVector.Zero, 5, 200, (AnimationLoopType)LoopType.Once).Encode().TrimEnd(Environment.NewLine.ToCharArray()),
                Is.EqualTo("Animation,Background,TopLeft,\"test.png\",0,0,5,200,LoopOnce")
            );
        }

        [Test]
        public void TestSampleEncoding()
        {
            Assert.That(
                new ScriptedSample(null, Layer.Background, "test.wav", 1000, 100).Encode(),
                Is.EqualTo("Sample,1000,Background,\"test.wav\",100")
            );
        }

        [Test]
        public void TestVideoEncoding()
        {
            Assert.That(
                new ScriptedVideo(null, "test.mp4", 400).Encode(),
                Is.EqualTo("Video,400,\"test.mp4\"")
            );
        }

        private static void createSpriteTimelineTest(Action<ScriptedSprite> action, string expected)
        {
            var sprite = new ScriptedSprite(null, Layer.Background, "test.png", osuAnchor.TopLeft, osuVector.Zero);
            action?.Invoke(sprite);
            Assert.That(string.Join('\n', sprite.Encode().Split('\n').Skip(1)).Replace("\r", string.Empty).Trim('\n'), Is.EqualTo(expected));
        }
    }
}
