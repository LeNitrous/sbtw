// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    public class DummyProject : IProject
    {
        public BindableInt Precision { get; }
        public GroupCollection Groups { get; }
        public BeatmapProvider Beatmaps { get; }
    }
}
