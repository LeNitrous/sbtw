// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Lists;
using osu.Game.Storyboards;
using osuTK;
using osuTK.Graphics;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Storyboards
{
    public class StoryboardEncoder : ScriptRunner<StringBuilder>
    {
        protected readonly Dictionary<Layer, StringBuilder> Layers = new Dictionary<Layer, StringBuilder>();

        protected readonly StringBuilder Samples = new StringBuilder();

        protected override StringBuilder CreateContext() => new StringBuilder();

        protected override void PreGenerate(StringBuilder context)
        {
            context.AppendLine("[Events]");
            context.AppendLine("// Background and Video Events");

            Layers.Clear();

            foreach (var layer in Enum.GetValues<Layer>())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"// Storyboard Layer {(int)layer} ({Enum.GetName(layer)})");
                Layers.Add(layer, builder);
            }

            Samples.AppendLine("// Storyboard Sound Samples");
        }

        protected override void HandleAnimation(StringBuilder context, ScriptedAnimation animation)
            => Layers[animation.Layer].AppendLine(handle_sprite(animation, $"Animation,{Enum.GetName(animation.Layer)},{Enum.GetName(animation.Origin)},\"{animation.Path}\",{animation.InitialPosition.X},{animation.InitialPosition.Y},{animation.FrameCount},{animation.FrameDelay},{Enum.GetName(animation.LoopType)}"));

        protected override void HandleSample(StringBuilder context, ScriptedSample sample)
            => Samples.AppendLine($"Sample,{sample.StartTime},{(int)sample.Layer},\"{sample.Path}\",{sample.Volume}");

        protected override void HandleSprite(StringBuilder context, ScriptedSprite sprite)
            => Layers[sprite.Layer].AppendLine(handle_sprite(sprite, $"Sprite,{Enum.GetName(sprite.Layer)},{Enum.GetName(sprite.Origin)},\"{sprite.Path}\",{sprite.InitialPosition.X},{sprite.InitialPosition.Y}"));

        protected override void HandleVideo(StringBuilder context, ScriptedVideo video)
            => context.AppendLine($"Video,{video.StartTime},\"{video.Path}\"");

        protected override void PostGenerate(StringBuilder context)
        {
            foreach ((var _, var builder) in Layers)
                context.Append(builder);

            context.Append(Samples);
        }

        private static string handle_sprite(ScriptedSprite sprite, string header)
        {
            var builder = new StringBuilder();

            builder.AppendLine(header);
            handle_timeline_group(builder, sprite.Timeline);

            foreach (var loop in sprite.Loops)
            {
                builder.AppendLine($" L,{loop.LoopStartTime},{loop.TotalIterations}");
                handle_timeline_group(builder, loop, 2);
            }

            foreach (var trigger in sprite.Triggers)
            {
                builder.AppendLine($" T,{trigger.TriggerName},{trigger.TriggerStartTime},{trigger.TriggerEndTime}");
                handle_timeline_group(builder, trigger, 2);
            }

            return builder.ToString();
        }

        private static void handle_timeline_group(StringBuilder builder, IScriptedCommandTimelineGroup group, int depth = 1)
        {
            var commands = new SortedList<TimelineCommand>();
            commands.AddRange(group.Move.Commands.Select(cmd => new TimelineCommand("M", cmd, format_vector)));
            commands.AddRange(group.X.Commands.Select(cmd => new TimelineCommand("MX", cmd)));
            commands.AddRange(group.Y.Commands.Select(cmd => new TimelineCommand("MY", cmd)));
            commands.AddRange(group.Alpha.Commands.Select(cmd => new TimelineCommand("F", cmd)));
            commands.AddRange(group.Colour.Commands.Select(cmd => new TimelineCommand("C", cmd, format_color)));
            commands.AddRange(group.FlipH.Commands.Select(cmd => new TimelineCommand("P", cmd, _ => "H")));
            commands.AddRange(group.FlipV.Commands.Select(cmd => new TimelineCommand("P", cmd, _ => "V")));
            commands.AddRange(group.BlendingParameters.Commands.Select(cmd => new TimelineCommand("P", cmd, _ => "A")));
            commands.AddRange(group.Scale.Commands.Select(cmd => new TimelineCommand("S", cmd)));
            commands.AddRange(group.VectorScale.Commands.Select(cmd => new TimelineCommand("V", cmd, format_vector)));

            foreach (var command in commands)
            {
                var formatFunc = command.FormatFunc ?? format;

                string stringCommand = command.Command.StartTime == command.Command.EndTime
                    ? $"{new string(' ', depth)}{command.Identifier},{(int)command.Command.Easing},{command.Command.StartTime},,"
                    : $"{new string(' ', depth)}{command.Identifier},{(int)command.Command.Easing},{command.Command.StartTime},{command.Command.EndTime},";

                if (command.Command is CommandTimeline<Vector2>.TypedCommand vectorCommand)
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
            if (value is not Vector2 vector)
                return null;

            return $"{vector.X},{vector.Y}";
        }

        private static string format_color(object value)
        {
            if (value is not Color4 color)
                return null;

            return $"{color.R * 255},{color.G * 255},{color.B * 255}";
        }

        private static string format(object value) => value.ToString();
    }
}
