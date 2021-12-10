// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Storyboards;
using sbtw.Common.Scripting;
using sbtw.Game.Projects;

namespace sbtw.Game.Scripting
{
    public class StoryboardGenerator : ScriptRunner<Storyboard>
    {
        public IReadOnlyDictionary<IStoryboardElement, IScriptedElement> ElementMap => elementMap;

        private readonly Dictionary<IStoryboardElement, IScriptedElement> elementMap = new Dictionary<IStoryboardElement, IScriptedElement>();
        private readonly BeatmapInfo beatmapInfo;

        public StoryboardGenerator(Project project, BeatmapInfo beatmapInfo)
            : base(project)
        {
            this.beatmapInfo = beatmapInfo;
        }

        protected override Storyboard CreateContext() => new Storyboard { BeatmapInfo = beatmapInfo };

        protected override void PreGenerate(Storyboard _) => elementMap.Clear();

        protected override void HandleAnimation(Storyboard context, ScriptedAnimation animation)
            => add(context, animation, copy(animation, new StoryboardAnimation(animation.Path, animation.Origin, animation.InitialPosition, animation.FrameCount, animation.FrameDelay, animation.LoopType)));

        protected override void HandleSprite(Storyboard context, ScriptedSprite sprite)
            => add(context, sprite, copy(sprite, new StoryboardSprite(sprite.Path, sprite.Origin, sprite.InitialPosition)));

        protected override void HandleSample(Storyboard context, ScriptedSample sample)
            => add(context, sample, new StoryboardSampleInfo(sample.Path, sample.Time, sample.Volume));

        protected override void HandleVideo(Storyboard context, ScriptedVideo video)
            => add(context, "Video", video, new StoryboardVideo(video.Path, video.Offset));

        private void add(Storyboard storyboard, IScriptedElement scripted, IStoryboardElement element)
            => add(storyboard, Enum.GetName(scripted.Layer), scripted, element);

        private void add(Storyboard storyboard, string layer, IScriptedElement scripted, IStoryboardElement element)
        {
            storyboard.GetLayer(layer).Add(element);
            elementMap.Add(element, scripted);
        }

        private static TObject copy<TScript, TObject>(TScript source, TObject destination)
            where TScript : ScriptedSprite
            where TObject : StoryboardSprite
        {
            foreach (var loop in source.Loops)
                copyTimelineGroups(loop, destination.AddLoop(loop.StartTime, loop.TotalIterations));

            foreach (var trigger in source.Triggers)
                copyTimelineGroups(trigger, destination.AddTrigger(trigger.TriggerName, trigger.StartTime, trigger.EndTime, trigger.GroupNumber));

            copyTimelineGroups(source.Timeline, destination.TimelineGroup);

            return destination;
        }

        private static void copyTimelineGroups(CommandTimelineGroup source, CommandTimelineGroup destination)
        {
            copyTimelines(source.X, destination.X);
            copyTimelines(source.Y, destination.Y);
            copyTimelines(source.Alpha, destination.Alpha);
            copyTimelines(source.Scale, destination.Scale);
            copyTimelines(source.FlipV, destination.FlipV);
            copyTimelines(source.FlipH, destination.FlipH);
            copyTimelines(source.Colour, destination.Colour);
            copyTimelines(source.Rotation, destination.Rotation);
            copyTimelines(source.VectorScale, destination.VectorScale);
            copyTimelines(source.BlendingParameters, destination.BlendingParameters);
        }

        private static void copyTimelines<TValue>(CommandTimeline<TValue> source, CommandTimeline<TValue> destination)
        {
            foreach (var command in source.Commands)
                destination.Add(command.Easing, command.StartTime, command.EndTime, command.StartValue, command.EndValue);
        }
    }
}
