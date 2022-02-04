// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using Newtonsoft.Json;
using osu.Framework.Bindables;

namespace sbtw.Editor.Projects
{
    public class ElementGroupSetting
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("visible")]
        public Bindable<bool> Visible { get; } = new Bindable<bool>(true);

        [JsonProperty("difficulty")]
        public Bindable<bool> ExportToDifficulty { get; } = new Bindable<bool>();
    }
}
