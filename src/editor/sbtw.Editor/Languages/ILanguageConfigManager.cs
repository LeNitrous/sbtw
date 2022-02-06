// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Languages
{
    public interface ILanguageConfigManager
    {
        TValue Get<TValue>(string key);
        void Set<TValue>(string key, TValue value);
    }
}
