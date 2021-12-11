// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Bindables;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;

namespace sbtw.Game.Projects
{
    public class DummyProject : IProject
    {
        public string Name => @"No project";
        public string Path => string.Empty;
        public string BeatmapPath => string.Empty;
        public bool UseStablePath => false;
        public Storage Storage => null;
        public IBeatmapSetInfo BeatmapSet => workingBeatmap.BeatmapSetInfo;
        public IResourceStore<byte[]> Resources => null;
        public BindableList<string> Groups => null;
        public Bindable<bool> ShowBeatmapBackground => null;
        public Bindable<bool> WidescreenStoryboard => null;

#pragma warning disable CS0067

        public event Action<ProjectFileType> FileChanged;

#pragma warning restore CS0067

        private readonly WorkingBeatmap workingBeatmap;

        public DummyProject(WorkingBeatmap workingBeatmap)
        {
            this.workingBeatmap = workingBeatmap;
        }

        public void Build(Action exitAction = null)
        {
            exitAction?.Invoke();
        }

        public void Clean(Action exitAction = null)
        {
            exitAction?.Invoke();
        }

        public void Restore(Action exitAction = null)
        {
            exitAction?.Invoke();
        }

        public WorkingBeatmap GetWorkingBeatmap() => GetWorkingBeatmap(string.Empty);

        public WorkingBeatmap GetWorkingBeatmap(string version) => workingBeatmap;

        public void Save()
        {
        }
    }
}
