// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics.Containers;
using osuTK;

namespace sbtw.Editor.Graphics.Containers
{
    public class AspectRatioPreservingContainer : Container
    {
        public new Vector2 Size => base.Size;
        protected override Vector2 DrawScale => new Vector2(Parent.DrawHeight / 480);

        public AspectRatioPreservingContainer()
        {
            base.Size = new Vector2(854, 480);
        }
    }
}
