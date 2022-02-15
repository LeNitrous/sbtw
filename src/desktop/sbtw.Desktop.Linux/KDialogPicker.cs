// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Humanizer;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Linux
{
    public class KDialogPicker : LinuxPicker
    {
        protected override string GetOpenFileCommand(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple)
            => $@"kdialog --getopenfilename ""{suggestedPath}"" ""{filters.Select(f => f.Description).Humanize()} ({string.Join(' ', filters.SelectMany(f => f.Files))}"")";

        protected override string GetOpenFolderCommand(string title, string suggestedPath)
            => $@"kdialog --getexistingdirectory ""{suggestedPath}""";

        protected override string GetSaveFileCommand(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters)
            => $@"kdialog --getsavefilename ""{Path.Combine(suggestedPath, suggestedFileName)}"" ""{filters.Select(f => f.Description).Humanize()} ({string.Join(' ', filters.SelectMany(f => f.Files))}""";
    }
}
