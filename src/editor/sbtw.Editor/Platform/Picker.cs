// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sbtw.Editor.Platform
{
    public abstract class Picker : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public async Task<IEnumerable<string>> OpenFileAsync(IReadOnlyList<PickerFilter> filters = null, bool allowMultiple = false)
            => (await OpenFileAsync("Open...", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filters, allowMultiple)) ?? Array.Empty<string>();

        public Task<string> OpenFolderAsync()
            => OpenFolderAsync("Choose...", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        public Task<string> SaveFileAsync(string suggestedFileName, IReadOnlyList<PickerFilter> filters)
            => SaveFileAsync("Save...", suggestedFileName, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filters);

        protected abstract Task<IEnumerable<string>> OpenFileAsync(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple);
        protected abstract Task<string> OpenFolderAsync(string title, string suggestedPath);
        protected abstract Task<string> SaveFileAsync(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters);

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
                IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
