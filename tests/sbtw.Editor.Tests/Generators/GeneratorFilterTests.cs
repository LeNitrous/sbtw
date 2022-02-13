// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using NUnit.Framework;
using sbtw.Editor.Generators;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;
using sbtw.Editor.Tests.Projects;
using sbtw.Editor.Tests.Scripts;

namespace sbtw.Editor.Tests.Generators
{
    public class GeneratorFilterTests
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
            var generated = new TestGenerator(provider).Generate(target: generatorTarget);

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
            var generated = new TestGenerator(provider).Generate(includeHidden: includeHidden);

            Assert.That(generated.Result.Count, Is.EqualTo(expectedCount));
        }

        private class TestGenerator : Generator<List<IScriptElement>, IScriptElement>
        {
            public TestGenerator(IProject project)
                : base(project)
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
