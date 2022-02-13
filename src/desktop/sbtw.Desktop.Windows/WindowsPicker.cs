// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using sbtw.Desktop.Windows.Extensions;
using sbtw.Desktop.Windows.Helpers;
using sbtw.Editor.Platform;

namespace sbtw.Desktop.Windows
{
    public class WindowsPicker : Picker
    {
        private WindowsWindowWrapper window;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private FolderBrowserDialog folderBrowserDialog;

        public IntPtr WindowHandle
        {
            get => window.Handle;
            set => window = new WindowsWindowWrapper { Handle = value };
        }

        protected override Task<IEnumerable<string>> OpenFileAsync(string title, string suggestedPath, IReadOnlyList<PickerFilter> filters, bool allowMultiple)
        {
            openFileDialog ??= new OpenFileDialog();
            openFileDialog.Title = title;
            openFileDialog.Multiselect = allowMultiple;
            openFileDialog.Filter = filters.ToFilterString();
            openFileDialog.CheckPathExists = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.InitialDirectory = suggestedPath;


            return STATask.Start<IEnumerable<string>>(() =>
            {
                var files = new List<string>();

                if (openFileDialog.ShowDialog(window) == DialogResult.OK)
                {
                    if (allowMultiple)
                        files.AddRange(openFileDialog.FileNames);
                    else
                        files.Add(openFileDialog.FileName);
                }

                return files;
            });
        }

        protected override Task<string> OpenFolderAsync(string title, string suggestedPath)
        {
            folderBrowserDialog ??= new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.UseDescriptionForTitle = true;
            folderBrowserDialog.Description = title;
            folderBrowserDialog.InitialDirectory = suggestedPath;

            return STATask.Start(() =>
            {
                string path = null;

                if (folderBrowserDialog.ShowDialog(window) == DialogResult.OK)
                    path = folderBrowserDialog.SelectedPath;

                return path;
            });
        }

        protected override Task<string> SaveFileAsync(string title, string suggestedFileName, string suggestedPath, IReadOnlyList<PickerFilter> filters)
        {
            saveFileDialog ??= new SaveFileDialog();
            saveFileDialog.Title = title;
            saveFileDialog.Filter = filters.ToFilterString();
            saveFileDialog.InitialDirectory = suggestedPath;
            saveFileDialog.CheckPathExists = true;

            return STATask.Start(() =>
            {
                string path = null;

                if (saveFileDialog.ShowDialog(window) == DialogResult.OK)
                    path = saveFileDialog.FileName;

                return path;
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            openFileDialog?.Dispose();
            saveFileDialog?.Dispose();
            folderBrowserDialog?.Dispose();
        }

        private struct WindowsWindowWrapper : IWin32Window
        {
            public IntPtr Handle { get; set; }
        }
    }
}
