// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using Newtonsoft.Json;
using osu.Framework.Bindables;

namespace sbtw.Game.Projects
{
    [Serializable]
    public class ProjectConfiguration
    {
        public string BeatmapPath
        {
            get => BeatmapPathBindable.Value;
            set => BeatmapPathBindable.Value = value;
        }


        [JsonIgnore]
        public string Path
        {
            get => PathBindable.Value;
            set => PathBindable.Value = value;
        }

        [JsonIgnore]
        public string Name
        {
            get => NameBindable.Value;
            set => NameBindable.Value = value;
        }

        public bool UseStablePath { get; set; }

        [JsonIgnore]
        public readonly Bindable<string> BeatmapPathBindable = new Bindable<string>();

        [JsonIgnore]
        public readonly Bindable<string> PathBindable = new Bindable<string>(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

        [JsonIgnore]
        public readonly Bindable<string> NameBindable = new Bindable<string>("new project");
    }
}
