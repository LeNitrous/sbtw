// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;
using sbtw.Editor.Studios;

namespace sbtw.Editor.Platform
{
    public abstract class DesktopEditor : Editor
    {
        public Picker Picker { get; private set; }
        public StudioManager Studios { get; private set; }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Picker = CreatePicker();
            Studios = new StudioManager(EditorConfig);
        }

        protected abstract Picker CreatePicker();

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            if (host.Window is not SDL2DesktopWindow window)
                return;

            window.CursorState |= CursorState.Hidden;
            window.Title = "sbtw!";

            window.SetIconFromStream(typeof(DesktopEditor).Assembly.GetManifestResourceStream("icon.ico"));
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            Picker?.Dispose();
        }
    }
}
