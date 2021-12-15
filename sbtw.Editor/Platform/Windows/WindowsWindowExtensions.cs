// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Runtime.InteropServices;
using osu.Framework.Platform.Windows;

namespace sbtw.Editor.Platform.Windows
{
    internal static class WindowsWindowExtensions
    {
        public static void EnableDarkMode(this WindowsWindow window, bool enabled)
        {
            int value = enabled ? 0x1 : 0x0;
            if (setWindowAttribute(window.WindowHandle, WindowAttribute.UseImmersiveDarkMode, ref value, Marshal.SizeOf<int>()) != HResult.OK)
                throw new Exception("Failed to set window to dark mode.");
        }

        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        private static extern HResult setWindowAttribute(IntPtr hwnd, WindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);
    }
}
