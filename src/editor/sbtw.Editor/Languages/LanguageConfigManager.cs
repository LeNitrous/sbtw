// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Configuration;

namespace sbtw.Editor.Languages
{
    public abstract class LanguageConfigManager<TLookup> : ConfigManager<TLookup>, ILanguageConfigManager
        where TLookup : struct, Enum
    {
        public LanguageConfigManager()
        {
            Load();
            InitialiseDefaults();
        }

        protected override void PerformLoad()
        {
        }

        protected override bool PerformSave()
        {
            return true;
        }
    }
}
