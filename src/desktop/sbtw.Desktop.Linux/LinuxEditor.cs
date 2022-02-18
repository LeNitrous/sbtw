// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Platform;

namespace sbtw.Desktop.Linux
{
    public class LinuxEditor : DesktopEditor
    {
        protected override Picker CreatePicker() => new NoOpPicker();
    }
}
