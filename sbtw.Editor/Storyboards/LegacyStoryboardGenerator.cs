// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Text;
using osu.Game.Storyboards;
using osuTK;
using osuTK.Graphics;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Storyboards
{
    public class LegacyStoryboardGenerator : ScriptRunner<StringBuilder>
    {
        private readonly Dictionary<Layer, StringBuilder> layers = new Dictionary<Layer, StringBuilder>();

        protected override StringBuilder CreateContext() => new StringBuilder();

        protected override void PreGenerate(StringBuilder context)
        {
            context.AppendLine("[Events]");
            context.AppendLine("// Background and Video Events");

            layers.Clear();

            foreach (var layer in Enum.GetValues<Layer>())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"// Storyboard Layer {layer} {Enum.GetName(layer)}");
                layers.Add(layer, builder);
            }
        }

        protected override void HandleAnimation(StringBuilder context, ScriptedAnimation animation)
            => layers[animation.Layer].AppendLine(handle_sprite(animation, $"Animation,{Enum.GetName(animation.Layer)},{Enum.GetName(animation.Origin)},\"{animation.Path}\",{animation.InitialPosition.X},{animation.InitialPosition.Y},{animation.FrameCount},{animation.FrameDelay},{Enum.GetName(animation.LoopType)}"));

        protected override void HandleSample(StringBuilder context, ScriptedSample sample)
            => layers[sample.Layer].AppendLine($"Sample,{sample.StartTime},{Enum.GetName(sample.Layer)},\"{sample.Path}\",{sample.Volume}");

        protected override void HandleSprite(StringBuilder context, ScriptedSprite sprite)
            => layers[sprite.Layer].AppendLine(handle_sprite(sprite, $"Sprite,{Enum.GetName(sprite.Layer)},{Enum.GetName(sprite.Origin)},\"{sprite.Path}\",{sprite.InitialPosition.X},{sprite.InitialPosition.Y}"));

        protected override void HandleVideo(StringBuilder context, ScriptedVideo video)
            => context.AppendLine($"Video,{video.StartTime},\"{video.Path}\"");

        protected override void PostGenerate(StringBuilder context)
        {
            foreach ((var _, var builder) in layers)
                context.AppendLine(builder.ToString());
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

        private static void handle_timeline_group(StringBuilder builder, CommandTimelineGroup group, int depth = 1)
        {
            handle_timeline(builder, group.X, "MX", depth);
            handle_timeline(builder, group.Y, "MY", depth);
            handle_timeline(builder, group.Scale, "S", depth);
            handle_timeline(builder, group.Alpha, "F", depth);
            handle_timeline(builder, group.Colour, "C", depth, handle_command_colour);
            handle_timeline(builder, group.VectorScale, "V", depth, handle_command_vector);
            handle_timeline(builder, group.FlipH, "P", depth, _ => "H");
            handle_timeline(builder, group.FlipV, "P", depth, _ => "V");
            handle_timeline(builder, group.BlendingParameters, "P", depth, _ => "A");
        }

        private static void handle_timeline<TValue>(StringBuilder builder, CommandTimeline<TValue> timeline, string identifier, int depth = 1, Func<CommandTimeline<TValue>.TypedCommand, string> format = null)
        {
            format ??= handle_command;
            foreach (var command in timeline.Commands)
                builder.AppendLine($"{new string(' ', depth)}{identifier},{(int)command.Easing},{command.StartTime},{command.EndTime},{format(command)}");
        }

        private static string handle_command_vector(CommandTimeline<Vector2>.TypedCommand command) => $"{command.StartValue.X},{command.StartValue.Y},{command.EndValue.X},{command.EndValue.Y}";
        private static string handle_command_colour(CommandTimeline<Color4>.TypedCommand command) => $"{command.StartValue.R},{command.StartValue.G},{command.StartValue.B},{command.EndValue.R},{command.EndValue.G},{command.EndValue.B}";
        private static string handle_command<TValue>(CommandTimeline<TValue>.TypedCommand command) => $"{command.StartValue},{command.EndValue}";
    }
}
