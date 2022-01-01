// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Languages.Javascript
{
    public class JavascriptConfigManager : LanguageConfigManager<JavascriptSetting>
    {
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
