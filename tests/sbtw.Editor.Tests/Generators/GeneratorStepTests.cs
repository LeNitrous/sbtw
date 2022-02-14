// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osuTK;
using sbtw.Editor.Generators;
using sbtw.Editor.Generators.Steps;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;
using sbtw.Editor.Tests.Projects;
using sbtw.Editor.Tests.Scripts;

namespace sbtw.Editor.Tests.Generators
{
    public class GeneratorStepTests
    {
        [TestCase(ExportTarget.None, ExportTarget.Storyboard, 0)]
        [TestCase(ExportTarget.Storyboard, ExportTarget.Storyboard, 1)]
        [TestCase(ExportTarget.Difficulty, ExportTarget.Storyboard, 0)]
        [TestCase(ExportTarget.Difficulty, null, 1)]
        public void TestGeneratorFilterTarget(ExportTarget groupTarget, ExportTarget? generatorTarget, int expectedCount)
        {
            var provider = new TestProject();

            var script = new TestScript
            {
                Action = s =>
                {
                    s.GetGroup("a").CreateSample("a", 100, 50, Layer.Foreground);
                }
            };

            provider.Groups.AddRange(new[]
            {
                new Group("a") { Target = { Value = groupTarget } }
            });

            (provider.Language as TestScriptLanguage).Scripts = new[] { script };

            var generated = new TestGenerator(provider)
                .AddStep(new FilterGroupStep(generatorTarget))
                .Generate(new ScriptGlobals { GroupProvider = provider });

            Assert.That(generated.Result.Count, Is.EqualTo(expectedCount));
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void TestGeneratorFilterVisibility(bool includeHidden, int expectedCount)
        {
            var provider = new TestProject();
            var script = new TestScript
            {
                Action = s =>
                {
                    s.GetGroup("a").CreateSample("a", 100, 50, Layer.Foreground);
                }
            };

            provider.Groups.AddRange(new[]
            {
                new Group("a") { Visible = { Value = false } }
            });

            (provider.Language as TestScriptLanguage).Scripts = new[] { script };

            var generated = new TestGenerator(provider)
                .AddStep(new FilterGroupStep(null, includeHidden))
                .Generate(new ScriptGlobals { GroupProvider = provider });

            Assert.That(generated.Result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void TestGeneratorPrecision()
        {
            float num = 0.7272727f;

            var provider = new TestProject();
            var script = new TestScript
            {
                Action = s =>
                {
                    Group group = s.GetGroup("a");
                    var sprite = group.CreateSprite("ab", position: new Vector2(num));
                    sprite.Move(0, 100, new Vector2(num), new Vector2(num));
                    sprite.Fade(0, 100, num, num);
                    sprite.MoveX(0, 100, num, num);
                    sprite.MoveY(0, 100, num, num);
                    sprite.Scale(0, 100, num, num);
                    sprite.Rotate(0, 100, num, num);
                    sprite.ScaleVec(0, 100, new Vector2(num), new Vector2(num));
                }
            };

            (provider.Language as TestScriptLanguage).Scripts = new[] { script };

            var generated = new TestGenerator(provider)
                .AddStep(new RoundToPrecisionStep())
                .Generate(new ScriptGlobals { GroupProvider = provider });

            Assert.That(generated.Result, Is.Not.Empty);
            Assert.That(generated.Result[0], Is.InstanceOf<ScriptedSprite>());

            var sprite = generated.Result[0] as ScriptedSprite;
            assertPrecision(sprite.Position.X, 4);
            assertPrecision(sprite.Position.Y, 4);

            var moveCommand = sprite.Timeline.Move.Commands.First();
            assertPrecision(moveCommand.StartValue.X, 4);
            assertPrecision(moveCommand.StartValue.Y, 4);
            assertPrecision(moveCommand.EndValue.X, 4);
            assertPrecision(moveCommand.EndValue.Y, 4);

            var fadeCommand = sprite.Timeline.Alpha.Commands.First();
            assertPrecision(fadeCommand.StartValue, 4);
            assertPrecision(fadeCommand.StartValue, 4);

            var moveXCommand = sprite.Timeline.X.Commands.First();
            assertPrecision(moveXCommand.StartValue, 4);
            assertPrecision(moveXCommand.StartValue, 4);

            var moveYCommand = sprite.Timeline.Y.Commands.First();
            assertPrecision(moveYCommand.StartValue, 4);
            assertPrecision(moveYCommand.StartValue, 4);

            var scaleCommand = sprite.Timeline.Scale.Commands.First();
            assertPrecision(scaleCommand.StartValue, 4);
            assertPrecision(scaleCommand.StartValue, 4);

            var rotateCommand = sprite.Timeline.Rotation.Commands.First();
            assertPrecision(rotateCommand.StartValue, 4);
            assertPrecision(rotateCommand.StartValue, 4);

            var vecScaleCommand = sprite.Timeline.VectorScale.Commands.First();
            assertPrecision(vecScaleCommand.StartValue.X, 4);
            assertPrecision(vecScaleCommand.StartValue.Y, 4);
            assertPrecision(vecScaleCommand.EndValue.X, 4);
            assertPrecision(vecScaleCommand.EndValue.Y, 4);

            static int getDecimalPlaces(float num) => num.ToString().Split('.').Last().Length;
            static void assertPrecision(float num, int amt) => Assert.That(getDecimalPlaces(num), Is.EqualTo(amt));
        }

        [Test]
        public void TestGeneratorWidescreenOffset()
        {
            var provider = new TestProject();
            var script = new TestScript
            {
                Action = s =>
                {
                    Group group = s.GetGroup("a");
                    var sprite = group.CreateSprite("ab", position: Vector2.Zero);
                    sprite.Move(0, 100, Vector2.Zero, Vector2.Zero);
                    sprite.MoveX(0, 100, 0, 0);
                }
            };

            (provider.Language as TestScriptLanguage).Scripts = new[] { script };

            var generated = new TestGenerator(provider)
                .AddStep(new OffsetForWidescreenStep())
                .Generate(new ScriptGlobals { GroupProvider = provider });

            Assert.That(generated.Result, Is.Not.Empty);
            Assert.That(generated.Result[0], Is.InstanceOf<ScriptedSprite>());

            var sprite = generated.Result[0] as ScriptedSprite;
            Assert.That(sprite.Position.X, Is.EqualTo(-107f));

            var moveCommand = sprite.Timeline.Move.Commands.First();
            var moveXCommand = sprite.Timeline.X.Commands.First();

            Assert.That(moveCommand.StartValue.X, Is.EqualTo(-107f));
            Assert.That(moveCommand.EndValue.X, Is.EqualTo(-107f));
            Assert.That(moveXCommand.EndValue, Is.EqualTo(-107f));
        }

        private class TestGenerator : Generator<List<IScriptElement>, IScriptElement>
        {
            public TestGenerator(ICanProvideScripts provider)
                : base(provider)
            {
            }

            protected override List<IScriptElement> CreateContext()
                => new List<IScriptElement>();

            protected override IScriptElement CreateAnimation(List<IScriptElement> context, ScriptedAnimation animation)
                => add(context, animation);

            protected override IScriptElement CreateSample(List<IScriptElement> context, ScriptedSample sample)
                => add(context, sample);

            protected override IScriptElement CreateSprite(List<IScriptElement> context, ScriptedSprite sprite)
                => add(context, sprite);

            protected override IScriptElement CreateVideo(List<IScriptElement> context, ScriptedVideo video)
                => add(context, video);

            private static IScriptElement add(List<IScriptElement> context, IScriptElement element)
            {
                context.Add(element);
                return element;
            }
        }
    }
}
