// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Beatmaps;
using osu.Game.Storyboards;
using sbtw.Editor.Storyboards;

namespace sbtw.Editor.Tests.Storyboards
{
    public class StoryboardGeneratorTest : ScriptRunnerTest<StoryboardGenerator, Storyboard>
    {
        protected override StoryboardGenerator CreateRunner() => new StoryboardGenerator(new BeatmapInfo());
    }
}
