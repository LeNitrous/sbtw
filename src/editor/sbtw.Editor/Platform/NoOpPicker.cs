// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sbtw.Editor.Platform
{
    public class NoOpPicker : Picker
    {
        protected override Task<IEnumerable<string>> OpenFileAsync(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple)
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }

        protected override Task<string> OpenFolderAsync(string title, string suggestedPath)
        {
            return Task.FromResult(string.Empty);
        }

        protected override Task<string> SaveFileAsync(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
