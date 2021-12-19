// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using System.Reflection;
using osu.Framework.Graphics;
using osu.Game.Storyboards;
using osuTK;
using osuTK.Graphics;

namespace sbtw.Editor.Scripts
{
    public class ScriptedCommandLoop : CommandLoop, IScriptedCommandTimelineGroup
    {
        public CommandTimeline<Vector2> Move = new CommandTimeline<Vector2>();

        public ScriptedCommandLoop(double startTime, int repeatCount)
            : base(startTime, repeatCount)
        {
            var timelinesField = typeof(CommandTimelineGroup).GetField("timelines", BindingFlags.Instance | BindingFlags.NonPublic);
            var timelines = timelinesField.GetValue(this) as ICommandTimeline[];
            timelinesField.SetValue(this, timelines.Concat(new[] { Move }).ToArray());
        }

        CommandTimeline<Vector2> IScriptedCommandTimelineGroup.Move => Move;
        CommandTimeline<float> IScriptedCommandTimelineGroup.X => X;
        CommandTimeline<float> IScriptedCommandTimelineGroup.Y => Y;
        CommandTimeline<float> IScriptedCommandTimelineGroup.Scale => Scale;
        CommandTimeline<Vector2> IScriptedCommandTimelineGroup.VectorScale => VectorScale;
        CommandTimeline<float> IScriptedCommandTimelineGroup.Rotation => Rotation;
        CommandTimeline<Color4> IScriptedCommandTimelineGroup.Colour => Colour;
        CommandTimeline<float> IScriptedCommandTimelineGroup.Alpha => Alpha;
        CommandTimeline<BlendingParameters> IScriptedCommandTimelineGroup.BlendingParameters => BlendingParameters;
        CommandTimeline<bool> IScriptedCommandTimelineGroup.FlipH => FlipH;
        CommandTimeline<bool> IScriptedCommandTimelineGroup.FlipV => FlipV;
    }
}
