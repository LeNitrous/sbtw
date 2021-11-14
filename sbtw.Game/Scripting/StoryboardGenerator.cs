// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using osu.Game.Storyboards;
using sbtw.Common.Scripting;
using sbtw.Game.Projects;

namespace sbtw.Game.Scripting
{
    public class StoryboardGenerator : ScriptAssemblyRunner<Storyboard>
    {
        public IReadOnlyDictionary<IStoryboardElement, IScriptedStoryboardElement> ElementMap => elementMap;

        private readonly Dictionary<IStoryboardElement, IScriptedStoryboardElement> elementMap = new Dictionary<IStoryboardElement, IScriptedStoryboardElement>();

        public StoryboardGenerator(Project project)
            : base(project)
        {
        }

        protected override Storyboard CreateContext() => new Storyboard { BeatmapInfo = { WidescreenStoryboard = true } };

        protected override void PreGenerate() => elementMap.Clear();

        protected override void HandleAnimation(Storyboard context, ScriptedStoryboardAnimation animation)
            => add(context, animation, apply(animation, new StoryboardAnimation(animation.Path, animation.Origin, animation.InitialPosition, animation.FrameCount, animation.FrameDelay, animation.LoopType)));

        protected override void HandleSprite(Storyboard context, ScriptedStoryboardSprite sprite)
            => add(context, sprite, apply(sprite, new StoryboardSprite(sprite.Path, sprite.Origin, sprite.InitialPosition)));

        protected override void HandleSample(Storyboard context, ScriptedStoryboardSample sample)
            => add(context, sample, new StoryboardSampleInfo(sample.Path, sample.Time, sample.Volume));

        protected override void HandleVideo(Storyboard context, ScriptedStoryboardVideo video)
            => add(context, "Video", video, new StoryboardVideo(video.Path, video.Offset));

        private void add(Storyboard storyboard, IScriptedStoryboardElement scripted, IStoryboardElement element)
            => add(storyboard, Enum.GetName(scripted.Layer), scripted, element);

        private void add(Storyboard storyboard, string layer, IScriptedStoryboardElement scripted, IStoryboardElement element)
        {
            storyboard.GetLayer(layer).Add(element);
            elementMap.Add(element, scripted);
        }

        private static TObject apply<TScript, TObject>(TScript source, TObject destination)
            where TScript : ScriptedStoryboardSprite
            where TObject : StoryboardSprite
        {
            foreach (var loop in source.Loops)
                destination.AddLoop(loop.StartTime, loop.TotalIterations);

            foreach (var trigger in source.Triggers)
                destination.AddTrigger(trigger.TriggerName, trigger.StartTime, trigger.EndTime, trigger.GroupNumber);

            destination.TimelineGroup.X = source.Timeline.X;
            destination.TimelineGroup.Y = source.Timeline.Y;
            destination.TimelineGroup.Alpha = source.Timeline.Alpha;
            destination.TimelineGroup.Scale = source.Timeline.Scale;
            destination.TimelineGroup.FlipH = source.Timeline.FlipH;
            destination.TimelineGroup.FlipV = source.Timeline.FlipV;
            destination.TimelineGroup.Rotation = source.Timeline.Rotation;
            destination.TimelineGroup.VectorScale = source.Timeline.VectorScale;
            destination.TimelineGroup.BlendingParameters = source.Timeline.BlendingParameters;

            return destination;
        }
    }
}
