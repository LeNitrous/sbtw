// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using System.Reflection;
using osu.Framework.Graphics;
using osu.Game.Storyboards;
using osuTK;
using osuTK.Graphics;

namespace sbtw.Editor.Scripts.Commands
{
    public class ScriptCommandTrigger : CommandTrigger, IScriptCommandTimelineGroup
    {
        public CommandTimeline<Vector2> Move = new CommandTimeline<Vector2>();

        public ScriptCommandTrigger(string triggerName, double startTime, double endTime, int groupNumber)
            : base(triggerName, startTime, endTime, groupNumber)
        {
            var timelinesField = typeof(CommandTimelineGroup).GetField("timelines", BindingFlags.Instance | BindingFlags.NonPublic);
            var timelines = timelinesField.GetValue(this) as ICommandTimeline[];
            timelinesField.SetValue(this, timelines.Concat(new[] { Move }).ToArray());
        }

        CommandTimeline<Vector2> IScriptCommandTimelineGroup.Move => Move;
        CommandTimeline<float> IScriptCommandTimelineGroup.X => X;
        CommandTimeline<float> IScriptCommandTimelineGroup.Y => Y;
        CommandTimeline<float> IScriptCommandTimelineGroup.Scale => Scale;
        CommandTimeline<Vector2> IScriptCommandTimelineGroup.VectorScale => VectorScale;
        CommandTimeline<float> IScriptCommandTimelineGroup.Rotation => Rotation;
        CommandTimeline<Color4> IScriptCommandTimelineGroup.Colour => Colour;
        CommandTimeline<float> IScriptCommandTimelineGroup.Alpha => Alpha;
        CommandTimeline<BlendingParameters> IScriptCommandTimelineGroup.BlendingParameters => BlendingParameters;
        CommandTimeline<bool> IScriptCommandTimelineGroup.FlipH => FlipH;
        CommandTimeline<bool> IScriptCommandTimelineGroup.FlipV => FlipV;
    }
}
