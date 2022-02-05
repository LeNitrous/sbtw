// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Game.Storyboards;

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

        [JsonIgnore]
        public readonly List<IStoryboardElement> Elements = new List<IStoryboardElement>();
    }

    public enum ExportTarget
    {
        Storyboard,
        Difficulty,
        None,
    }
}
