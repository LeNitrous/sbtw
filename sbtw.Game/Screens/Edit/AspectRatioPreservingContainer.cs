// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics.Containers;
using osuTK;

namespace sbtw.Game.Screens
{
    public class AspectRatioPreservingContainer : Container
    {
        protected override Vector2 DrawScale => new Vector2(Parent.DrawHeight / 480);

        public AspectRatioPreservingContainer()
        {
            Size = new Vector2(854, 480);
        }
    }
}
