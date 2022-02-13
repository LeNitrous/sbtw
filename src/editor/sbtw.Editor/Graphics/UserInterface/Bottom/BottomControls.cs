// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Screens.Edit;
using osuTK;

namespace sbtw.Editor.Graphics.UserInterface.Bottom
{
    public class BottomControls : FillFlowContainer
    {
        private readonly IBeatmap beatmap;
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        public BottomControls(IBeatmap beatmap)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Horizontal;
            Spacing = new Vector2(5);
            this.beatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            dependencies.CacheAs(new EditorBeatmap(beatmap));
            AddRange(new Drawable[]
            {
                new TimeDisplay(),
                new SeekControl(),
                new RateSelector(),
                new PlayButton(),
            });
        }
    }
}
