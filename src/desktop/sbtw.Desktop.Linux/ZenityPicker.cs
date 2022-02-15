// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Linux
{
    public class ZenityPicker : LinuxPicker
    {
        protected override string GetOpenFileCommand(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple)
            => $@"zenity --title=""{title}"" --filename=""{suggestedPath}""" + $@"{(allowMultiple ? " --multiple" : string.Empty)} --separator=""|""";

        protected override string GetOpenFolderCommand(string title, string suggestedPath)
            => $@"zenity --title=""{title}"" --filename=""{suggestedPath}"" --directory";

        protected override string GetSaveFileCommand(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters)
            => $@"zenity --title=""{title}"" --filename=""{suggestedPath}"" --save";
    }
}
