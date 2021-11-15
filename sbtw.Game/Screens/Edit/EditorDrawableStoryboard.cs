// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.OpenGL.Textures;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Screens.Edit;
using osu.Game.Storyboards;
using osu.Game.Storyboards.Drawables;
using osuTK;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Edit
{
    public class EditorDrawableStoryboard : Container<DrawableStoryboardLayer>
    {
        [Cached]
        public Storyboard Storyboard { get; }

        protected override Container<DrawableStoryboardLayer> Content { get; }

        protected override Vector2 DrawScale => new Vector2(Parent.DrawHeight / 480);

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        public EditorDrawableStoryboard(Storyboard storyboard)
        {
            Storyboard = storyboard;
            Size = new Vector2(854, 480);

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChild = Content = new Container<DrawableStoryboardLayer>
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        }

        [BackgroundDependencyLoader]
        private void load(EditorClock clock, GameHost host, Bindable<Project> project)
        {
            Clock = clock;

            dependencies.CacheAs<TextureStore>(new EditorDrawableStoryboardTextureStore(host.CreateTextureLoaderStore(project.Value.Resources)));

            foreach (var layer in Storyboard.Layers)
                Add(layer.CreateDrawable());
        }

        private class EditorDrawableStoryboardTextureStore : TextureStore
        {
            public EditorDrawableStoryboardTextureStore(IResourceStore<TextureUpload> store)
                : base(store, false, scaleAdjust: 1)
            {
            }

            public override Texture Get(string name, WrapMode wrapModeS, WrapMode wrapModeT)
            {
                return base.Get(name, wrapModeS, wrapModeT);
            }
        }
    }
}
