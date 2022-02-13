// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;
using osu.Framework.Platform.Windows;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Windows
{
    public class WindowsEditor : DesktopEditor
    {
        protected override Picker CreatePicker() => new WindowsPicker();

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            if (host.Window is not WindowsWindow window)
                return;

            (Picker as WindowsPicker).WindowHandle = window.WindowHandle;
        }
    }
}
