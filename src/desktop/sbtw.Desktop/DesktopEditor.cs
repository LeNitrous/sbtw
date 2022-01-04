// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Platform;
using sbtw.Desktop.IO;
using sbtw.Desktop.Studios;
using sbtw.Editor.Languages.Javascript;
using sbtw.Editor.Studios;

namespace sbtw.Desktop
{
    public class DesktopEditor : Editor.Editor
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Languages.Register(new JavascriptLanguage());
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            if (host.Window is not SDL2DesktopWindow window)
                return;

            window.CursorState |= CursorState.Hidden;
            window.Title = "sbtw!";

            window.SetIconFromStream(typeof(DesktopEditor).Assembly.GetManifestResourceStream("icon.ico"));
        }

        public override Task<IEnumerable<string>> RequestMultipleFileAsync(string title = "Open Files", string suggestedPath = null, IEnumerable<string> extensions = null)
            => Task.Run<IEnumerable<string>>(() => TinyFileDialog.OpenFile(title, extensions, string.Empty, suggestedPath, true).Split('|'));

        public override Task<string> RequestPathAsync(string title = "Open Folder", string suggestedPath = null)
            => Task.Run(() => TinyFileDialog.OpenFolder(title, suggestedPath));

        public override Task<string> RequestSaveFileAsync(string title = "Save File", string suggestedName = "file", string suggestedPath = null, IEnumerable<string> extensions = null)
            => Task.Run(() => TinyFileDialog.SaveFile(title, suggestedName, extensions, string.Empty, suggestedPath));

        public override Task<string> RequestSingleFileAsync(string title = "Open File", string suggestedPath = null, IEnumerable<string> extensions = null)
            => Task.Run(() => TinyFileDialog.OpenFile(title, extensions, string.Empty, suggestedPath, false));

        protected override StudioManager CreateStudioManager() => new DesktopStudioManager(LocalEditorConfig);
    }
}
