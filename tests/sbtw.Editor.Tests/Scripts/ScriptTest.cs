// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Storyboards;
using sbtw.Editor.Generators;
using sbtw.Editor.Tests.Generators;

namespace sbtw.Editor.Tests.Scripts
{
    public class ScriptTest : GeneratorTest<StoryboardGenerator, Storyboard, IStoryboardElement>
    {
        protected override StoryboardGenerator CreateRunner() => new StoryboardGenerator(new BeatmapInfo());

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
