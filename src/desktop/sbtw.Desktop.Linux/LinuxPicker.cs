// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading.Tasks;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Linux
{
    public abstract class LinuxPicker : Picker
    {
        protected sealed override Task<IEnumerable<string>> OpenFileAsync(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple)
        {
            throw new System.NotImplementedException();
        }

        protected sealed override Task<string> OpenFolderAsync(string title, string suggestedPath)
        {
            throw new System.NotImplementedException();
        }

        protected sealed override Task<string> SaveFileAsync(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters)
        {
            throw new System.NotImplementedException();
        }
    }
}
