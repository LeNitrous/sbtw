// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading.Tasks;
using sbtw.Editor.Studios;
using sbtw.Editor.Tests.Studios;

namespace sbtw.Editor.Tests
{
    public class TestEditor : EditorBase
    {
        public override Task<IEnumerable<string>> RequestMultipleFileAsync(string title = "Open Files", string suggestedPath = null, IEnumerable<string> extensions = null)
        {
            throw new System.NotImplementedException();
        }

        public override Task<string> RequestPathAsync(string title = "Open Folder", string suggestedPath = null)
        {
            throw new System.NotImplementedException();
        }

        public override Task<string> RequestSaveFileAsync(string title = "Save File", string suggestedName = "file", string suggestedPath = null, IEnumerable<string> extensions = null)
        {
            throw new System.NotImplementedException();
        }

        public override Task<string> RequestSingleFileAsync(string title = "Open File", string suggestedPath = null, IEnumerable<string> extensions = null)
        {
            throw new System.NotImplementedException();
        }

        protected override StudioManager CreateStudioManager() => new TestStudioManager(LocalEditorConfig);
    }
}
