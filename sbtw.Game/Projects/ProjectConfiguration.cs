// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Bindables;

namespace sbtw.Game.Projects
{
    public class ProjectConfiguration
    {
        public string BeatmapPath
        {
            get => BeatmapPathBindable.Value;
            set => BeatmapPathBindable.Value = value;
        }

        public string Path
        {
            get => PathBindable.Value;
            set => PathBindable.Value = value;
        }

        public string Name
        {
            get => NameBindable.Value;
            set => NameBindable.Value = value;
        }

        public bool UseStablePath { get; set; }

        public readonly Bindable<string> BeatmapPathBindable = new Bindable<string>();
        public readonly Bindable<string> PathBindable = new Bindable<string>(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        public readonly Bindable<string> NameBindable = new Bindable<string>("new project");
    }
}
