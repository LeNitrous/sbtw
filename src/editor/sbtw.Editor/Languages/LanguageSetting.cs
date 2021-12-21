// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using Realms;

namespace sbtw.Editor.Languages
{
    [MapTo(@"LanguageSetting")]
    public class LanguageSetting : RealmObject
    {
        [Indexed]
        public string LanguageName { get; set; } = string.Empty;

        [Required]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;

        public override string ToString() => $"{Key} => {Value}";
    }
}
