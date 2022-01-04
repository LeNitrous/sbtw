// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Game.Storyboards;
using osuTK;

namespace sbtw.Editor.Scripts.Elements
{
    public class ScriptedAnimation : ScriptedSprite
    {
        public int FrameCount { get; }

        public double FrameDelay { get; }

        public AnimationLoopType LoopType { get; }

        public ScriptedAnimation(Script owner, Layer layer, string path, Anchor origin, Vector2 initialPosition, int frameCount, double frameDelay, AnimationLoopType loopType)
            : base(owner, layer, path, origin, initialPosition)
        {
            FrameCount = frameCount;
            FrameDelay = frameDelay;
            LoopType = loopType;
        }
    }
}
