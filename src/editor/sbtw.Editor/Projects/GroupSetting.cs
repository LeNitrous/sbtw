// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using Newtonsoft.Json;
using osu.Framework.Bindables;

namespace sbtw.Editor.Projects
{
    public class GroupSetting
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hidden")]
        public BindableBool Hidden { get; } = new BindableBool();

        [JsonProperty("target")]
        public Bindable<ExportTarget> Target { get; } = new Bindable<ExportTarget>();
    }

    public enum ExportTarget
    {
        Storyboard,
        Difficulty,
        None,
    }
}
