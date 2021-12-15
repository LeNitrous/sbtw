// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Reflection;
using Microsoft.Win32;
using osu.Framework.Platform;
using osu.Framework.Platform.Windows;
using sbtw.Editor.Platform.Windows;
using sbtw.Editor.Scripts;
using sbtw.Editor.Studios;

namespace sbtw.Editor
{
    public class Editor : EditorBase
    {
        protected override IScriptRuntime CreateScriptRuntime() => new JSScriptRuntime(LocalEditorConfig);
        protected override IStudioManager CreateStudioManager() => new StudioManager(LocalEditorConfig);

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            if (host.Window is not SDL2DesktopWindow window)
                return;

            window.CursorState |= CursorState.Hidden;
            window.Title = "sbtw!";

            window.SetIconFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), "icon.ico"));

            if (OperatingSystem.IsWindows() && window is WindowsWindow windowsWindow)
            {
                int useLightMode = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
                windowsWindow.EnableDarkMode(useLightMode != 1);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            (ScriptRuntime as JSScriptRuntime)?.Dispose();
        }
    }
}
