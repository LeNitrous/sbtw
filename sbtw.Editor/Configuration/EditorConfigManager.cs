// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace sbtw.Editor.Configuration
{
    public class EditorConfigManager : IniConfigManager<EditorSetting>
    {
        protected override string Filename => "editor.ini";

        public EditorConfigManager(Storage storage)
            : base(storage)
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(EditorSetting.PreferredStudio, string.Empty);
            SetDefault(EditorSetting.HotReload, true);
            SetDefault(EditorSetting.Recents, string.Empty);
            SetDefault(EditorSetting.DebugPort, 7270);
        }
    }
}
