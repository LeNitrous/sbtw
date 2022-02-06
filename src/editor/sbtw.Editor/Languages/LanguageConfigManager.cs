// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Configuration;

namespace sbtw.Editor.Languages
{
    public class LanguageConfigManager : ILanguageConfigManager
    {
        private readonly ILanguage language;
        private readonly JsonBackedConfigManager config;

        public LanguageConfigManager(ILanguage language, JsonBackedConfigManager config)
        {
            this.config = config;
            this.language = language;
        }

        public TValue Get<TValue>(string key) => config.Get<TValue>($"{language.Name}.{key}");
        public void Set<TValue>(string key, TValue value) => config.Set($"{language.Name}.{key}", value);
    }
}
