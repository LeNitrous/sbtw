// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using osu.Game.Storyboards;
using osuAnchor = osu.Framework.Graphics.Anchor;
using osuVector = osuTK.Vector2;

namespace sbtw.Common.Scripting
{
    public class StoryboardScriptElementGroup
    {
        /// <summary>
        /// The name of this group.
        /// </summary>
        public readonly string Name;

        internal IEnumerable<IScriptedStoryboardElement> Elements
        {
            get => elements;
            set
            {
                Clear();
                AddRange(value);
            }
        }

        internal double StartTime => elements.OfType<IScriptedElementHasStartTime>().Min(s => s.StartTime);

        internal double EndTime => elements.OfType<IScriptedElementHasEndTime>().Max(s => s.EndTime);

        internal double Duration => EndTime - StartTime;

        private readonly List<IScriptedStoryboardElement> elements = new List<IScriptedStoryboardElement>();
        private readonly StoryboardScript owner;

        internal StoryboardScriptElementGroup(StoryboardScript owner, string name)
        {
            Name = name;
            this.owner = owner;
        }

        internal StoryboardScriptElementGroup(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Creates a new sprite for this group.
        /// </summary>
        public ScriptedStoryboardSprite CreateSprite(string path, Anchor origin = Anchor.TopLeft, Vector2 initialPosition = default, StoryboardLayerName layer = StoryboardLayerName.Background)
        {
            var sprite = new ScriptedStoryboardSprite(owner, layer, path, (osuAnchor)origin, new osuVector(initialPosition.X, initialPosition.Y));
            Add(sprite);
            return sprite;
        }

        /// <summary>
        /// Creates a new animation for this group.
        /// </summary>
        public ScriptedStoryboardAnimation CreateAnimation(string path, Anchor origin = Anchor.TopLeft, Vector2 initialPosition = default, int frameCount = 0, double frameDelay = 0, LoopType loopType = LoopType.Once, StoryboardLayerName layer = StoryboardLayerName.Background)
        {
            var animation = new ScriptedStoryboardAnimation(owner, layer, path, (osuAnchor)origin, new osuVector(initialPosition.X, initialPosition.Y), frameCount, frameDelay, (AnimationLoopType)loopType);
            Add(animation);
            return animation;
        }

        /// <summary>
        /// Creates a new sample for this group.
        /// </summary>
        public void CreateSample(string path, double time, int volume = 100, StoryboardLayerName layer = StoryboardLayerName.Background)
            => Add(new ScriptedStoryboardSample(owner, layer, path, time, volume));

        internal void CreateVideo(string path, int offset) => elements.Add(new ScriptedStoryboardVideo(owner, path, offset));

        internal void Add(IScriptedStoryboardElement element) => elements.Add(element);

        internal void Remove(IScriptedStoryboardElement element) => elements.Remove(element);

        internal void AddRange(IEnumerable<IScriptedStoryboardElement> elementsToAdd) => elements.AddRange(elementsToAdd);

        internal void Clear() => elements.Clear();
    }
}
