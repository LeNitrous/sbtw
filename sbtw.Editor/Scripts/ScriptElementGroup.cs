// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Storyboards;
using osuTK;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Storyboards;

namespace sbtw.Editor.Scripts
{
    public class ScriptElementGroup
    {
        public readonly string Name;

        public readonly Script Owner;

        public readonly List<IScriptedElement> Elements = new List<IScriptedElement>();

        public ScriptElementGroup(Script owner, string name)
        {
            Name = name;
            Owner = owner;
        }

        public ScriptElementGroup(string name, IEnumerable<IScriptedElement> elements)
            : this(null, name)
        {
            Elements.AddRange(elements);
        }

        public ScriptedSprite CreateSprite(string path, Anchor origin = Anchor.TopLeft, Vector2 initialPosition = default, Layer layer = Layer.Background)
        {
            var sprite = new ScriptedSprite(Owner, layer, path, origin, initialPosition);
            Elements.Add(sprite);
            return sprite;
        }

        public ScriptedAnimation CreateAnimation(string path, Anchor origin = Anchor.TopLeft, Vector2 initialPosition = default, int frameCount = 0, double frameDelay = 0, AnimationLoopType loopType = AnimationLoopType.LoopOnce, Layer layer = Layer.Background)
        {
            var animation = new ScriptedAnimation(Owner, layer, path, origin, initialPosition, frameCount, frameDelay, loopType);
            Elements.Add(animation);
            return animation;
        }

        public void CreateSample(string path, double time, int volume = 100, Layer layer = Layer.Background)
            => Elements.Add(new ScriptedSample(Owner, layer, path, time, volume));

        internal void CreateVideo(string path, int offset) => Elements.Add(new ScriptedVideo(Owner, path, offset));
    }
}
