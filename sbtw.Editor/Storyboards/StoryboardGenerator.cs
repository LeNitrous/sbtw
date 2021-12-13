// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Storyboards;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Storyboards
{
    public class StoryboardGenerator : ScriptRunner<Storyboard>
    {
        public IReadOnlyDictionary<IStoryboardElement, IScriptedElement> Elements => elements;

        private readonly Dictionary<IStoryboardElement, IScriptedElement> elements = new Dictionary<IStoryboardElement, IScriptedElement>();
        private readonly BeatmapInfo beatmapInfo;

        public StoryboardGenerator(BeatmapInfo beatmapInfo)
        {
            this.beatmapInfo = beatmapInfo;
        }

        protected override Storyboard CreateContext() => new Storyboard { BeatmapInfo = beatmapInfo };

        protected override void HandleAnimation(Storyboard context, ScriptedAnimation animation)
            => add(context, animation, copy(animation, new StoryboardAnimation(animation.Path, animation.Origin, animation.InitialPosition, animation.FrameCount, animation.FrameDelay, animation.LoopType)));

        protected override void HandleSample(Storyboard context, ScriptedSample sample)
            => add(context, sample, new StoryboardSampleInfo(sample.Path, sample.StartTime, sample.Volume));

        protected override void HandleSprite(Storyboard context, ScriptedSprite sprite)
            => add(context, sprite, copy(sprite, new StoryboardSprite(sprite.Path, sprite.Origin, sprite.InitialPosition)));

        protected override void HandleVideo(Storyboard context, ScriptedVideo video)
            => add(context, "Video", video, new StoryboardVideo(video.Path, (int)video.StartTime));

        private void add(Storyboard storyboard, IScriptedElement scripted, IStoryboardElement element)
            => add(storyboard, Enum.GetName(scripted.Layer), scripted, element);

        private void add(Storyboard storyboard, string layer, IScriptedElement scripted, IStoryboardElement element)
        {
            storyboard.GetLayer(layer).Add(element);
            elements.Add(element, scripted);
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

        private static void copy_timeline_groups(IScriptedCommandTimelineGroup source, CommandTimelineGroup destination)
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
