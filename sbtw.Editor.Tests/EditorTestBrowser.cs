// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;
using osu.Framework.Testing;

namespace sbtw.Editor.Tests
{
    public class EditorTestBrowser : TestEditor
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();
            Add(new TestBrowser("sbtw"));
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
