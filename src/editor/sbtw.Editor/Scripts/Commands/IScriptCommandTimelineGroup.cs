// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Game.Storyboards;
using osuTK;
using osuTK.Graphics;

namespace sbtw.Editor.Scripts.Commands
{
    public interface IScriptCommandTimelineGroup
    {
        CommandTimeline<Vector2> Move { get; }
        CommandTimeline<float> X { get; }
        CommandTimeline<float> Y { get; }
        CommandTimeline<float> Scale { get; }
        CommandTimeline<Vector2> VectorScale { get; }
        CommandTimeline<float> Rotation { get; }
        CommandTimeline<Color4> Colour { get; }
        CommandTimeline<float> Alpha { get; }
        CommandTimeline<BlendingParameters> BlendingParameters { get; }
        CommandTimeline<bool> FlipH { get; }
        CommandTimeline<bool> FlipV { get; }
    }
}
