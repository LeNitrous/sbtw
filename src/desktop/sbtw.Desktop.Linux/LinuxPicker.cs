// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading.Tasks;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Linux
{
    public abstract class LinuxPicker : Picker
    {
        protected sealed override async Task<IEnumerable<string>> OpenFileAsync(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple)
        {
            return (await LinuxHelper.ExecuteAsync(GetOpenFileCommand(title, suggestedPath, filters, allowMultiple))).Split('|');
        }

        protected sealed override async Task<string> OpenFolderAsync(string title, string suggestedPath)
        {
            return await LinuxHelper.ExecuteAsync(GetOpenFolderCommand(title, suggestedPath));
        }

        protected sealed override async Task<string> SaveFileAsync(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters)
        {
            return await LinuxHelper.ExecuteAsync(GetSaveFileCommand(title, suggestedFileName, suggestedPath, filters));
        }

        protected abstract string GetOpenFileCommand(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple);
        protected abstract string GetOpenFolderCommand(string title, string suggestedPath);
        protected abstract string GetSaveFileCommand(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters);
    }
}
