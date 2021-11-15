// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using osu.Framework.Graphics;
using osu.Game.Storyboards;
using osuTK.Graphics;
using osuAnchor = osu.Framework.Graphics.Anchor;
using osuEasing = osu.Framework.Graphics.Easing;
using osuVector = osuTK.Vector2;

namespace sbtw.Common.Scripting
{
    public class ScriptedSprite : ScriptedElement, IScriptedElementHasEndTime
    {
        internal string Path { get; private set; }

        internal osuAnchor Origin { get; private set; }

        internal osuVector InitialPosition { get; private set; }

        internal IReadOnlyList<CommandLoop> Loops => loops;

        internal IReadOnlyList<CommandTrigger> Triggers => triggers;

        internal readonly CommandTimelineGroup Timeline = new CommandTimelineGroup();

        private CommandTimelineGroup context;

        private CommandTimelineGroup currentContext
        {
            get => context ?? Timeline;
            set => context = value;
        }
        public double EndTime => Timeline.CommandsEndTime;

        public double Duration => Timeline.CommandsDuration;

        public double StartTime => Timeline.CommandsStartTime;

        private readonly List<CommandLoop> loops = new List<CommandLoop>();
        private readonly List<CommandTrigger> triggers = new List<CommandTrigger>();

        public ScriptedSprite(Script owner, Layer layer, string path, osuAnchor origin, osuVector initialPosition)
            : base(owner, layer)
        {
            Path = path;
            Origin = origin;
            InitialPosition = initialPosition;
        }

        /// <summary>
        /// Changes the position of the sprite overtime in both X and Y axes.
        /// </summary>
        /// <remarks>
        /// See <see cref="MoveX"/> and <see cref="MoveY"/> in moving the sprite's position in a single axis.
        /// </remarks>
        public void Move(Easing easing, double startTime, double endTime, Vector2 startPosition, Vector2 endPosition)
        {
            MoveX(easing, startTime, endTime, startPosition.X, endPosition.X);
            MoveY(easing, startTime, endTime, startPosition.Y, endPosition.Y);
        }

        /// <inheritdoc cref="Move"/>
        public void Move(Easing easing, double startTime, double endTime, Vector2 startPosition, float endX, float endY)
            => Move(easing, startTime, endTime, startPosition, new Vector2(endX, endY));

        /// <inheritdoc cref="Move"/>
        public void Move(Easing easing, double startTime, double endTime, float startX, float startY, Vector2 endPosition)
            => Move(easing, startTime, endTime, new Vector2(startX, startY), endPosition);

        /// <inheritdoc cref="Move"/>
        public void Move(Easing easing, double startTime, double endTime, float startX, float startY, float endX, float endY)
            => Move(easing, startTime, endTime, new Vector2(startX, startY), new Vector2(endX, endY));

        /// <inheritdoc cref="Move"/>
        public void Move(double startTime, double endTime, Vector2 startPosition, Vector2 endPosition)
            => Move(Easing.None, startTime, endTime, startPosition, endPosition);

        /// <inheritdoc cref="Move"/>
        public void Move(double startTime, double endTime, Vector2 startPosition, float endX, float endY)
            => Move(Easing.None, startTime, endTime, startPosition, new Vector2(endX, endY));

        /// <inheritdoc cref="Move"/>
        public void Move(double startTime, double endTime, float startX, float startY, Vector2 endPosition)
            => Move(Easing.None, startTime, endTime, new Vector2(startX, startY), endPosition);

        /// <inheritdoc cref="Move"/>
        public void Move(double startTime, double endTime, float startX, float startY, float endX, float endY)
            => Move(Easing.None, startTime, endTime, new Vector2(startX, startY), new Vector2(endX, endY));

        /// <inheritdoc cref="Move"/>
        public void Move(double time, Vector2 position)
            => Move(Easing.None, time, time, position, position);

        /// <inheritdoc cref="Move"/>
        public void Move(double time, float x, float y)
            => Move(time, new Vector2(x, y));

        /// <summary>
        /// Changes the position of the sprite overtime in the X axis.
        /// </summary>
        /// <remarks>
        /// See <see cref="MoveY"/> in moving the sprite in the Y axis and <see cref="Move"/> in moving the sprite in both axes.
        /// </remarks>
        public void MoveX(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.X.Add((osuEasing)easing, startTime, endTime, start, end);

        /// <inheritdoc cref="MoveX"/>
        public void MoveX(double startTime, double endTime, float start, float end)
            => MoveX(Easing.None, startTime, endTime, start, end);

        /// <inheritdoc cref="MoveX"/>
        public void MoveX(double time, float x)
            => MoveX(time, time, x, x);

        /// <summary>
        /// Changes the position of the sprite overtime in the Y axis.
        /// </summary>
        /// <remarks>
        /// See <see cref="MoveX"/> in moving the sprite in the X axis and <see cref="Move"/> in moving the sprite in both axes.
        /// </remarks>
        public void MoveY(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.Y.Add((osuEasing)easing, startTime, endTime, start, end);

        /// <inheritdoc cref="MoveY"/>
        public void MoveY(double startTime, double endTime, float start, float end)
            => MoveY(Easing.None, startTime, endTime, start, end);

        /// <inheritdoc cref="MoveY"/>
        public void MoveY(double time, float x)
            => MoveY(time, time, x, x);

        /// <summary>
        /// Changes the sprite's size proportionally overtime. See <see cref="ScaleVec"/> in changing the sprite's size in individual axes.
        /// </summary>
        public void Scale(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.Scale.Add((osuEasing)easing, startTime, endTime, start, end);

        /// <inheritdoc cref="Scale"/>
        public void Scale(double startTime, double endTime, float start, float end)
            => Scale(Easing.None, startTime, endTime, start, end);

        /// <inheritdoc cref="Scale"/>
        public void Scale(double time, float x)
            => Scale(time, time, x, x);

        // <summary>
        /// Changes the sprite's size overtime. See <see cref="Scale"/> in changing the sprite's size proportionally.
        /// </summary>
        public void ScaleVec(Easing easing, double startTime, double endTime, Vector2 startScale, Vector2 endScale)
            => currentContext.VectorScale.Add((osuEasing)easing, startTime, endTime, new osuVector(startScale.X, startScale.Y), new osuVector(endScale.X, endScale.Y));

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(Easing easing, double startTime, double endTime, Vector2 startPosition, float endX, float endY)
            => ScaleVec(easing, startTime, endTime, startPosition, new Vector2(endX, endY));

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(Easing easing, double startTime, double endTime, float startX, float startY, Vector2 endPosition)
            => ScaleVec(easing, startTime, endTime, new Vector2(startX, startY), endPosition);

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(Easing easing, double startTime, double endTime, float startX, float startY, float endX, float endY)
            => ScaleVec(easing, startTime, endTime, new Vector2(startX, startY), new Vector2(endX, endY));

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(double startTime, double endTime, Vector2 startPosition, Vector2 endPosition)
            => ScaleVec(Easing.None, startTime, endTime, startPosition, endPosition);

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(double startTime, double endTime, Vector2 startPosition, float endX, float endY)
            => ScaleVec(Easing.None, startTime, endTime, startPosition, new Vector2(endX, endY));

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(double startTime, double endTime, float startX, float startY, Vector2 endPosition)
            => ScaleVec(Easing.None, startTime, endTime, new Vector2(startX, startY), endPosition);

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(double startTime, double endTime, float startX, float startY, float endX, float endY)
            => ScaleVec(Easing.None, startTime, endTime, new Vector2(startX, startY), new Vector2(endX, endY));

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(double time, Vector2 position)
            => ScaleVec(Easing.None, time, time, position, position);

        /// <inheritdoc cref="ScaleVec"/>
        public void ScaleVec(double time, float x, float y)
            => ScaleVec(time, new Vector2(x, y));

        /// <summary>
        /// Changes the sprite's rotation overtime.
        /// </summary>
        public void Rotate(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.Rotation.Add((osuEasing)easing, startTime, endTime, start, end);

        /// <inheritdoc cref="Rotate"/>
        public void Rotate(double startTime, double endTime, float start, float end)
            => Rotate(Easing.None, startTime, endTime, start, end);

        /// <inheritdoc cref="Rotate"/>
        public void Rotate(double time, float x)
            => Rotate(time, time, x, x);

        /// <summary>
        /// Changes the sprite's opacity overtime.
        /// </summary>
        public void Fade(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.Alpha.Add((osuEasing)easing, startTime, endTime, start, end);

        /// <inheritdoc cref="Fade"/>
        public void Fade(double startTime, double endTime, float start, float end)
            => Fade(Easing.None, startTime, endTime, start, end);

        /// <inheritdoc cref="Fade"/>
        public void Fade(double time, float x)
            => Fade(time, time, x, x);

        /// <summary>
        /// Changes the sprite's color overtime.
        /// </summary>
        public void Color(Easing easing, double startTime, double endTime, Color startColor, Color endColor)
            => currentContext.Colour.Add((osuEasing)easing, startTime, endTime, (Colour4)startColor, (Colour4)endColor);

        /// <inheritdoc cref="Color"/>
        public void Color(Easing easing, double startTime, double endTime, Color startColor, float endRed, float endBlue, float endGreen)
            => Color(easing, startTime, endTime, startColor, new Color(endRed, endBlue, endGreen));

        /// <inheritdoc cref="Color"/>
        public void Color(Easing easing, double startTime, double endTime, float startRed, float startBlue, float startGreen, Color endColor)
            => Color(easing, startTime, endTime, new Color(startRed, startBlue, startGreen), endColor);

        /// <inheritdoc cref="Color"/>
        public void Color(Easing easing, double startTime, double endTime, float startRed, float startBlue, float startGreen, float endRed, float endBlue, float endGreen)
            => Color(easing, startTime, endTime, new Color(startRed, startBlue, startGreen), new Color(endRed, endBlue, endGreen));

        /// <inheritdoc cref="Color"/>
        public void Color(double startTime, double endTime, Color startColor, Color endColor)
            => Color(Easing.None, startTime, endTime, startColor, endColor);

        /// <inheritdoc cref="Color"/>
        public void Color(double startTime, double endTime, Color startColor, float endRed, float endBlue, float endGreen)
            => Color(Easing.None, startTime, endTime, startColor, new Color(endRed, endBlue, endGreen));

        /// <inheritdoc cref="Color"/>
        public void Color(double startTime, double endTime, float startRed, float startBlue, float startGreen, Color endColor)
            => Color(Easing.None, startTime, endTime, new Color(startRed, startBlue, startGreen), endColor);

        /// <inheritdoc cref="Color"/>
        public void Color(double startTime, double endTime, float startRed, float startBlue, float startGreen, float endRed, float endBlue, float endGreen)
            => Color(Easing.None, startTime, endTime, new Color(startRed, startBlue, startGreen), new Color(endRed, endBlue, endGreen));

        /// <inheritdoc cref="Color"/>
        public void Color(double time, Color color)
            => Color(Easing.None, time, time, color, color);

        /// <inheritdoc cref="Color"/>
        public void Color(double time, float red, float blue, float green)
            => Color(time, new Color(red, blue, green));

        /// <summary>
        /// Flips the sprite horizontally.
        /// </summary>
        public void FlipH(Easing easing, double startTime, double endTime)
            => currentContext.FlipH.Add((osuEasing)easing, startTime, endTime, true, startTime == endTime);

        /// <inheritdoc cref="FlipH"/>
        public void FlipH(double time)
            => FlipH(Easing.None, time, time);

        /// <summary>
        /// Flips the sprite vertically.
        /// </summary>
        public void FlipV(Easing easing, double startTime, double endTime)
            => currentContext.FlipV.Add((osuEasing)easing, startTime, endTime, true, startTime == endTime);

        /// <inheritdoc cref="FlipV"/>
        public void FlipV(double time)
            => FlipV(Easing.None, time, time);

        /// <summary>
        /// Applies an additive blending to the sprite. Best used in conjunction with <see cref="Color"/>.
        /// </summary>
        public void Additive(Easing easing, double startTime, double endTime)
            => currentContext.BlendingParameters.Add((osuEasing)easing, startTime, endTime, BlendingParameters.Additive, startTime == endTime ? BlendingParameters.Additive : BlendingParameters.Inherit);

        /// <inheritdoc cref="Additive"/>
        public void Additive(double time)
            => Additive(Easing.None, time, time);

        /// <summary>
        /// Repeats commands called after this until <see cref="EndGroup"/> is called.
        /// </summary>
        public void StartLoopGroup(double startTime, int repeatCount)
        {
            if (context != null)
                throw new InvalidOperationException("Cannot start a new group when an existing group is active.");

            var loop = new CommandLoop(startTime, repeatCount - 1);
            loops.Add(loop);
            currentContext = loop;
        }

        /// <summary>
        /// Performs a set of commands called after this until <see cref="EndGroup"/> is called.
        /// </summary>
        public void StartTriggerGroup(string triggerName, double startTime, double endTime, int group)
        {
            if (context != null)
                throw new InvalidOperationException("Cannot start a new group when an existing group is active.");

            var trigger = new CommandTrigger(triggerName, startTime, endTime, group);
            triggers.Add(trigger);
            currentContext = trigger;
        }

        /// <summary>
        /// Ends the current group.
        /// </summary>
        public void EndGroup()
        {
            currentContext = null;
        }

        protected internal virtual string Header
            => $"{GetType().Name.Replace("Scripted", string.Empty)},{Enum.GetName(Layer)},{Enum.GetName(Origin)},\"{Path}\",{InitialPosition.X},{InitialPosition.Y}";

        internal override string Encode()
        {
            var builder = new StringBuilder();

            builder.AppendLine(Header);
            handleTimelineGroup(builder, Timeline);

            foreach (var loop in Loops)
            {
                builder.AppendLine($" L,{loop.LoopStartTime},{loop.TotalIterations}");
                handleTimelineGroup(builder, loop, 2);
            }

            foreach (var trigger in Triggers)
            {
                builder.AppendLine($" T,{trigger.TriggerName},{trigger.TriggerStartTime},{trigger.TriggerEndTime}");
                handleTimelineGroup(builder, trigger, 2);
            }

            return builder.ToString();
        }

        private static void handleTimelineGroup(StringBuilder builder, CommandTimelineGroup group, int depth = 1)
        {
            handleTimeline(builder, group.X, "MX", depth);
            handleTimeline(builder, group.Y, "MY", depth);
            handleTimeline(builder, group.Scale, "S", depth);
            handleTimeline(builder, group.Alpha, "F", depth);
            handleTimeline(builder, group.Colour, "C", depth, handleCommandColor);
            handleTimeline(builder, group.VectorScale, "V", depth, handleCommandVector);
            handleTimeline(builder, group.FlipH, "P", depth, _ => "H");
            handleTimeline(builder, group.FlipV, "P", depth, _ => "V");
            handleTimeline(builder, group.BlendingParameters, "P", depth, _ => "A");
        }

        private static void handleTimeline<TValue>(StringBuilder builder, CommandTimeline<TValue> timeline, string identifier, int depth = 1, Func<CommandTimeline<TValue>.TypedCommand, string> format = null)
        {
            format ??= handleCommand;
            foreach (var command in timeline.Commands)
                builder.AppendLine($"{new string(' ', depth)}{identifier},{(int)command.Easing},{command.StartTime},{command.EndTime},{format(command)}");
        }

        private static string handleCommandVector(CommandTimeline<osuVector>.TypedCommand command) => $"{command.StartValue.X},{command.StartValue.Y},{command.EndValue.X},{command.EndValue.Y}";
        private static string handleCommandColor(CommandTimeline<Color4>.TypedCommand command) => $"{command.StartValue.R},{command.StartValue.G},{command.StartValue.B},{command.EndValue.R},{command.EndValue.G},{command.EndValue.B}";
        private static string handleCommand<TValue>(CommandTimeline<TValue>.TypedCommand command) => $"{command.StartValue},{command.EndValue}";
    }
}
