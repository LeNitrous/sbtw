// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;

namespace sbtw.Editor.Configuration
{
    public class DevelopmentEditorConfigManager : EditorConfigManager
    {
        protected override string Filename => "editor-dev.ini";

        public DevelopmentEditorConfigManager(Storage storage)
            : base(storage)
        {
        }
    }
}
