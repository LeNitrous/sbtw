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
    public class ScriptElementGroup
    {
        /// <summary>
        /// The name of this group.
        /// </summary>
        public readonly string Name;

        internal IEnumerable<IScriptedElement> Elements
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

        private readonly List<IScriptedElement> elements = new List<IScriptedElement>();
        private readonly Script owner;

        internal ScriptElementGroup(Script owner, string name)
        {
            Name = name;
            this.owner = owner;
        }

        internal ScriptElementGroup(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Creates a new sprite for this group.
        /// </summary>
        public ScriptedSprite CreateSprite(string path, Anchor origin = Anchor.TopLeft, Vector2 initialPosition = default, Layer layer = Layer.Background)
        {
            var sprite = new ScriptedSprite(owner, layer, path, (osuAnchor)origin, new osuVector(initialPosition.X, initialPosition.Y));
            Add(sprite);
            return sprite;
        }

        /// <summary>
        /// Creates a new animation for this group.
        /// </summary>
        public ScriptedAnimation CreateAnimation(string path, Anchor origin = Anchor.TopLeft, Vector2 initialPosition = default, int frameCount = 0, double frameDelay = 0, LoopType loopType = LoopType.Once, Layer layer = Layer.Background)
        {
            var animation = new ScriptedAnimation(owner, layer, path, (osuAnchor)origin, new osuVector(initialPosition.X, initialPosition.Y), frameCount, frameDelay, (AnimationLoopType)loopType);
            Add(animation);
            return animation;
        }

        /// <summary>
        /// Creates a new sample for this group.
        /// </summary>
        public void CreateSample(string path, double time, int volume = 100, Layer layer = Layer.Background)
            => Add(new ScriptedSample(owner, layer, path, time, volume));

        internal void CreateVideo(string path, int offset) => elements.Add(new ScriptedVideo(owner, path, offset));

        internal void Add(IScriptedElement element) => elements.Add(element);

        internal void Remove(IScriptedElement element) => elements.Remove(element);

        internal void AddRange(IEnumerable<IScriptedElement> elementsToAdd) => elements.AddRange(elementsToAdd);

        internal void Clear() => elements.Clear();
    }
}
