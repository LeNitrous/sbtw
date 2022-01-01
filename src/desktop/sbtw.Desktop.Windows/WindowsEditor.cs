// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using osu.Framework.Platform;
using osu.Framework.Platform.Windows;
using osu.Game.Extensions;
using Windows.UI;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace sbtw.Desktop.Windows
{
    public class WindowsEditor : DesktopEditor
    {
        protected AppWindow AppWindow { get; private set; }
        protected new WindowsWindow Window => base.Window as WindowsWindow;

        public override async Task<IEnumerable<string>> RequestMultipleFileAsync(string title = "Open Files", string suggestedPath = null, IEnumerable<string> extensions = null)
            => (await initializeFileOpenPicker(extensions).PickMultipleFilesAsync()).Select(file => file.Path);

        public override async Task<string> RequestSingleFileAsync(string title = "Open File", string suggestedPath = null, IEnumerable<string> extensions = null)
            => (await initializeFileOpenPicker(extensions).PickSingleFileAsync())?.Path ?? string.Empty;

        private FileOpenPicker initializeFileOpenPicker(IEnumerable<string> extensions)
        {
            var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            picker.FileTypeFilter.AddRange(extensions ?? new[] { "*" });

            InitializeWithWindow.Initialize(picker, Window.WindowHandle);
            return picker;
        }

        public override async Task<string> RequestSaveFileAsync(string title = "Save File", string suggestedName = "file", string suggestedPath = null, IEnumerable<string> extensions = null)
        {
            var picker = new FileSavePicker
            {
                SuggestedFileName = suggestedName,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };

            InitializeWithWindow.Initialize(picker, Window.WindowHandle);
            return (await picker.PickSaveFileAsync())?.Path ?? string.Empty;
        }

        public override async Task<string> RequestPathAsync(string title = "Open Folder", string suggestedPath = null)
        {
            var picker = new FolderPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            InitializeWithWindow.Initialize(picker, Window.WindowHandle);
            return (await picker.PickSingleFolderAsync())?.Path ?? string.Empty;
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            if (Window == null)
                return;

            var windowId = Win32Interop.GetWindowIdFromWindow(Window.WindowHandle);
            AppWindow = AppWindow.GetFromWindowId(windowId);

            AppWindow.TitleBar.BackgroundColor = Color.FromArgb(255, 17, 17, 17);
            AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(255, 17, 17, 17);
            AppWindow.TitleBar.InactiveBackgroundColor = Color.FromArgb(255, 17, 17, 17);
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 17, 17, 17);
        }
    }
}
