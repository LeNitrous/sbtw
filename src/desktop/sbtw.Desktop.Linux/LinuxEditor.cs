// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Linux
{
    public class LinuxEditor : DesktopEditor
    {
        protected override Picker CreatePicker()
        {
            if (!string.IsNullOrEmpty(LinuxHelper.Execute("which kdialog")))
                return new KDialogPicker();

            if (!string.IsNullOrEmpty(LinuxHelper.Execute("which zenity")))
                return new ZenityPicker();

            throw new PlatformNotSupportedException("No picker can be made.");
        }
    }
}
