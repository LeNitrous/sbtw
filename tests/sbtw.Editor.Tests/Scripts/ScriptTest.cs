// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Storyboards;
using sbtw.Editor.Storyboards;

namespace sbtw.Editor.Tests.Scripts
{
    public class ScriptTest : GeneratorTest<StoryboardGenerator, Storyboard, IStoryboardElement>
    {
        protected override StoryboardGenerator CreateRunner() => new StoryboardGenerator(new BeatmapInfo());

        [Test]
        public void TestVariableSet()
        {
            var generated = Generate("TestScript", script => script.SetValue("TestVariable", true));
            Assert.That(generated.Variables["TestScript"]["TestVariable"], Is.True);
        }

        [Test]
        public void TestVariableSetImmutable()
        {
            var generated = Generate("TestScript", script =>
            {
                script.SetValue("TestVariable", 44);
                script.SetValue("TestVariable", 89);
            });

            Assert.That(generated.Variables["TestScript"]["TestVariable"], Is.EqualTo(44));
        }

        [Test]
        public void TestVariableSetProtected()
        {
            var generated = Generate(
                "TestScript",
                script => script.SetValue("TestVariable", 1),
                new Dictionary<string, IReadOnlyDictionary<string, object>>
                {
                    {
                        "TestScript",
                        new Dictionary<string, object>
                        {
                            {
                                "TestVariable",
                                24
                            }
                        }
                    }
                });

            Assert.That(generated.Variables["TestScript"]["TestVariable"], Is.EqualTo(24));
        }

        [Test]
        public void TestVariableGet()
        {
            bool variable = false;

            var generated = Generate("TestScript", script =>
            {
                script.SetValue("TestVariable", true);
                variable = script.GetValue<bool>("TestVariable");
            });

            Assert.That(variable, Is.True);
        }

        [Test]
        public void TestGroupOrdering()
        {
            var preferred = new[]
            {
                "Bravo",
                "Alpha",
                "Delta",
                "Charlie"
            };

            var generated = Generate(script =>
            {
                script.GetGroup("Alpha");
                script.GetGroup("Bravo");
                script.GetGroup("Charlie");
                script.GetGroup("Delta");
            }, ordering: preferred);

            Assert.That(Enumerable.SequenceEqual(preferred, generated.Groups), Is.True);
        }
    }
}
