// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using Realms;

namespace sbtw.Editor.Scripts
{
    [MapTo(@"ScriptEnvironmentSetting")]
    public class ScriptEnvironmentSetting : RealmObject
    {
        [Indexed]
        public string EnvironmentName { get; set; } = string.Empty;

        [Required]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;

        public override string ToString() => $"{Key} => {Value}";
    }
}
