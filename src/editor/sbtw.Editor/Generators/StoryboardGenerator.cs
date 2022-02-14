// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Game.Storyboards;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts.Commands;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Generators
{
    public class StoryboardGenerator : Generator<Storyboard, IStoryboardElement>
    {
        public StoryboardGenerator(ICanProvideScripts provider)
            : base(provider)
        {
        }

        protected override Storyboard CreateContext() => new Storyboard();

        private static IStoryboardElement add(Storyboard storyboard, Layer layer, IStoryboardElement element)
            => add(storyboard, Enum.GetName(layer), element);

        protected override IStoryboardElement CreateAnimation(Storyboard context, ScriptedAnimation animation)
            => add(context, animation.Layer, copy(animation, new StoryboardAnimation(animation.Path, animation.Origin, animation.Position, animation.FrameCount, animation.FrameDelay, animation.LoopType)));

        protected override IStoryboardElement CreateSample(Storyboard context, ScriptedSample sample)
            => add(context, sample.Layer, new StoryboardSampleInfo(sample.Path, sample.StartTime, sample.Volume));

        protected override IStoryboardElement CreateSprite(Storyboard context, ScriptedSprite sprite)
            => add(context, sprite.Layer, copy(sprite, new StoryboardSprite(sprite.Path, sprite.Origin, sprite.Position)));

        protected override IStoryboardElement CreateVideo(Storyboard context, ScriptedVideo video)
            => add(context, "Video", new StoryboardVideo(video.Path, (int)video.StartTime));

        private static IStoryboardElement add(Storyboard storyboard, string layer, IStoryboardElement element)
        {
            storyboard.GetLayer(layer).Add(element);
            return element;
        }

        private static TObject copy<TScript, TObject>(TScript source, TObject destination)
            where TScript : ScriptedSprite
            where TObject : StoryboardSprite
        {
            foreach (var loop in source.Loops)
                copy_timeline_groups(loop, destination.AddLoop(loop.StartTime, loop.TotalIterations));

            foreach (var trigger in source.Triggers)
                copy_timeline_groups(trigger, destination.AddTrigger(trigger.TriggerName, trigger.StartTime, trigger.EndTime, trigger.GroupNumber));

            copy_timeline_groups(source.Timeline, destination.TimelineGroup);

            return destination;
        }

        private static void copy_timeline_groups(IScriptCommandTimelineGroup source, CommandTimelineGroup destination)
        {
            foreach (var sourceMove in source.Move.Commands)
            {
                destination.X.Add(sourceMove.Easing, sourceMove.StartTime, sourceMove.EndTime, sourceMove.StartValue.X, sourceMove.EndValue.X);
                destination.Y.Add(sourceMove.Easing, sourceMove.StartTime, sourceMove.EndTime, sourceMove.StartValue.Y, sourceMove.EndValue.Y);
            }

            copy_timelines(source.X, destination.X);
            copy_timelines(source.Y, destination.Y);
            copy_timelines(source.Alpha, destination.Alpha);
            copy_timelines(source.Scale, destination.Scale);
            copy_timelines(source.FlipV, destination.FlipV);
            copy_timelines(source.FlipH, destination.FlipH);
            copy_timelines(source.Colour, destination.Colour);
            copy_timelines(source.Rotation, destination.Rotation);
            copy_timelines(source.VectorScale, destination.VectorScale);
            copy_timelines(source.BlendingParameters, destination.BlendingParameters);
        }

        private static void copy_timelines<TValue>(CommandTimeline<TValue> source, CommandTimeline<TValue> destination)
        {
            foreach (var command in source.Commands)
                destination.Add(command.Easing, command.StartTime, command.EndTime, command.StartValue, command.EndValue);
        }
    }
}
