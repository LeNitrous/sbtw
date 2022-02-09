// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Game.Storyboards;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Scripts.Elements
{
    public class ScriptedAnimation : ScriptedSprite
    {
        public int FrameCount { get; }
        public double FrameDelay { get; }
        public AnimationLoopType LoopType { get; }

        public ScriptedAnimation(Group group, string path, Layer layer, Vector2 position, Anchor origin, int frameCount, double frameDelay, AnimationLoopType loopType)
            : base(group, path, layer, position, origin)
        {
            FrameCount = frameCount;
            FrameDelay = frameDelay;
            LoopType = loopType;
        }
    }
}
