// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Database;

namespace sbtw.Editor.Languages.Javascript
{
    public class JavascriptConfigManager : LanguageConfigManager<JavascriptSetting>
    {
        public JavascriptConfigManager(ILanguage language, RealmContextFactory context)
            : base(language, context)
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(JavascriptSetting.DebugEnabled, false);
            SetDefault(JavascriptSetting.DebugPort, 7270);
        }
    }

    public enum JavascriptSetting
    {
        DebugEnabled,
        DebugPort,
    }
}
