// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Lists;
using osu.Game.Storyboards;
using osuTK.Graphics;
using sbtw.Editor.Scripts.Commands;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Generators
{
    public class OsbGenerator : Generator<Dictionary<string, StringBuilder>, StringBuilder>
    {
        protected override Dictionary<string, StringBuilder> CreateContext() => new Dictionary<string, StringBuilder>();

        protected override void PreGenerate(Dictionary<string, StringBuilder> context)
        {
            foreach (var layer in Enum.GetValues<Layer>())
                context.Add(Enum.GetName(layer), new StringBuilder());

            context.Add("Samples", new StringBuilder());
            context.Add("Video", new StringBuilder());
        }

        protected override StringBuilder CreateAnimation(Dictionary<string, StringBuilder> context, ScriptedAnimation animation)
            => context[animation.Layer.ToString()].AppendLine(encode_sprite(animation, $"Animation,{Enum.GetName(animation.Layer)},{Enum.GetName(animation.Origin)},\"{animation.Path}\",{animation.Position.X:0},{animation.Position.Y:0},{animation.FrameCount},{animation.FrameDelay},{Enum.GetName(animation.LoopType)}"));

        protected override StringBuilder CreateSample(Dictionary<string, StringBuilder> context, ScriptedSample sample)
            => context["Samples"].AppendLine($"Sample,{sample.StartTime},{(int)sample.Layer},\"{sample.Path}\",{sample.Volume}");

        protected override StringBuilder CreateSprite(Dictionary<string, StringBuilder> context, ScriptedSprite sprite)
            => context[sprite.Layer.ToString()].AppendLine(encode_sprite(sprite, $"Sprite,{Enum.GetName(sprite.Layer)},{Enum.GetName(sprite.Origin)},\"{sprite.Path}\",{sprite.Position.X:0},{sprite.Position.Y:0}"));

        protected override StringBuilder CreateVideo(Dictionary<string, StringBuilder> context, ScriptedVideo video)
            => context["Video"].AppendLine($"Video,{video.StartTime},\"{video.Path}\"");

        private static string encode_sprite(ScriptedSprite sprite, string header)
        {
            var builder = new StringBuilder();

            builder.AppendLine(header);
            encode_timeline_group(builder, sprite.Timeline);

            foreach (var loop in sprite.Loops)
            {
                builder.AppendLine($" L,{loop.LoopStartTime},{loop.TotalIterations}");
                encode_timeline_group(builder, loop, 2);
            }

            foreach (var trigger in sprite.Triggers)
            {
                builder.AppendLine($" T,{trigger.TriggerName},{trigger.TriggerStartTime:0},{trigger.TriggerEndTime:0}");
                encode_timeline_group(builder, trigger, 2);
            }

            return builder.ToString();
        }

        private static void encode_timeline_group(StringBuilder builder, IScriptCommandTimelineGroup group, int depth = 1)
        {
            var commands = new SortedList<TimelineCommand>();
            commands.AddRange(group.Move.Commands.Select(cmd => new TimelineCommand("M", cmd, format_vector)));
            commands.AddRange(group.X.Commands.Select(cmd => new TimelineCommand("MX", cmd, format_float_no_decimal)));
            commands.AddRange(group.Y.Commands.Select(cmd => new TimelineCommand("MY", cmd, format_float_no_decimal)));
            commands.AddRange(group.Alpha.Commands.Select(cmd => new TimelineCommand("F", cmd, format_float)));
            commands.AddRange(group.Colour.Commands.Select(cmd => new TimelineCommand("C", cmd, format_color)));
            commands.AddRange(group.FlipH.Commands.Select(cmd => new TimelineCommand("P", cmd, _ => "H")));
            commands.AddRange(group.FlipV.Commands.Select(cmd => new TimelineCommand("P", cmd, _ => "V")));
            commands.AddRange(group.BlendingParameters.Commands.Select(cmd => new TimelineCommand("P", cmd, _ => "A")));
            commands.AddRange(group.Scale.Commands.Select(cmd => new TimelineCommand("S", cmd, format_float)));
            commands.AddRange(group.Rotation.Commands.Select(cmd => new TimelineCommand("R", cmd, format_float)));
            commands.AddRange(group.VectorScale.Commands.Select(cmd => new TimelineCommand("V", cmd, format_vector)));

            foreach (var command in commands)
            {
                var formatFunc = command.FormatFunc ?? format;

                string stringCommand = command.Command.StartTime == command.Command.EndTime
                    ? $"{new string(' ', depth)}{command.Identifier},{(int)command.Command.Easing},{command.Command.StartTime:0},,"
                    : $"{new string(' ', depth)}{command.Identifier},{(int)command.Command.Easing},{command.Command.StartTime:0},{command.Command.EndTime:0},";

                if (command.Command is CommandTimeline<osuTK.Vector2>.TypedCommand vectorCommand)
                {
                    stringCommand += vectorCommand.StartValue.Equals(vectorCommand.EndValue)
                        ? formatFunc(vectorCommand.StartValue)
                        : $"{formatFunc(vectorCommand.StartValue)},{formatFunc(vectorCommand.EndValue)}";
                }

                if (command.Command is CommandTimeline<Color4>.TypedCommand colorCommand)
                {
                    stringCommand += colorCommand.StartValue.Equals(colorCommand.EndValue)
                        ? formatFunc(colorCommand.StartValue)
                        : $"{formatFunc(colorCommand.StartValue)},{formatFunc(colorCommand.EndValue)}";
                }

                if (command.Command is CommandTimeline<float>.TypedCommand floatCommand)
                {
                    stringCommand += floatCommand.StartValue.Equals(floatCommand.EndValue)
                        ? formatFunc(floatCommand.StartValue)
                        : $"{formatFunc(floatCommand.StartValue)},{formatFunc(floatCommand.EndValue)}";
                }

                if (command.Command is CommandTimeline<bool>.TypedCommand boolCommand)
                    stringCommand += formatFunc(boolCommand.StartValue);

                if (command.Command is CommandTimeline<BlendingParameters>.TypedCommand additiveCommand)
                    stringCommand += formatFunc(additiveCommand.StartValue);

                builder.AppendLine(stringCommand);
            }
        }

        private struct TimelineCommand : IComparable<TimelineCommand>
        {
            public string Identifier { get; set; }

            public ICommand Command { get; set; }

            public Func<object, string> FormatFunc { get; set; }

            public TimelineCommand(string identifier, ICommand command, Func<object, string> formatFunc = null)
            {
                Identifier = identifier;
                FormatFunc = formatFunc;
                Command = command;
            }

            public int CompareTo(TimelineCommand other)
                => Command.CompareTo(other.Command);
        }

        private static string format_vector(object value)
        {
            if (value is not osuTK.Vector2 vector)
                return null;

            return $"{vector.X:0},{vector.Y:0}";
        }

        private static string format_color(object value)
        {
            if (value is not Color4 color)
                return null;

            return $"{color.R:0},{color.G:0},{color.B:0}";
        }

        private static string format_float(object value)
        {
            if (value is not float floatValue)
                return null;

            return (floatValue % 1) > float.Epsilon ? floatValue.ToString("0.00") : floatValue.ToString("0");
        }

        private static string format_float_no_decimal(object value)
        {
            if (value is not float floatValue)
                return null;

            return floatValue.ToString("0");
        }

        private static string format(object value) => value.ToString();
    }
}
