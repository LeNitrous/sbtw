// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Storyboards;
using osuTK;
using osuAnchor = osu.Framework.Graphics.Anchor;

namespace sbtw.Common.Scripting
{
    public class ScriptedStoryboardAnimation : ScriptedStoryboardSprite
    {
        public int FrameCount { get; private set; }

        public double FrameDelay { get; private set; }

        public AnimationLoopType LoopType { get; private set; }

        public ScriptedStoryboardAnimation(StoryboardScript owner, StoryboardLayerName layer, string path, osuAnchor origin, Vector2 initialPosition, int frameCount, double frameDelay, AnimationLoopType loopType)
            : base(owner, layer, path, origin, initialPosition)
        {
            FrameCount = frameCount;
            FrameDelay = frameDelay;
            LoopType = loopType;
        }
    }
}
