// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;

namespace sbtw.Editor
{
    public abstract class Editor : EditorBase
    {
        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            if (host.Window is not SDL2DesktopWindow window)
                return;

            window.CursorState |= CursorState.Hidden;
            window.Title = "sbtw!";
        }
    }
}
