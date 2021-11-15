// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Game.Storyboards;
using osuTK;
using osuAnchor = osu.Framework.Graphics.Anchor;

namespace sbtw.Common.Scripting
{
    public class ScriptedAnimation : ScriptedSprite
    {
        internal int FrameCount { get; private set; }

        internal double FrameDelay { get; private set; }

        internal AnimationLoopType LoopType { get; private set; }

        public ScriptedAnimation(Script owner, Layer layer, string path, osuAnchor origin, Vector2 initialPosition, int frameCount, double frameDelay, AnimationLoopType loopType)
            : base(owner, layer, path, origin, initialPosition)
        {
            FrameCount = frameCount;
            FrameDelay = frameDelay;
            LoopType = loopType;
        }

        protected internal override string Header => $"{base.Header},{FrameCount},{FrameDelay},{Enum.GetName(LoopType)}";
    }
}
