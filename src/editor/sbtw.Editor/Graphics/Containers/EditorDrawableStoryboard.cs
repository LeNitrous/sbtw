// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Screens.Edit;
using osu.Game.Storyboards;
using osu.Game.Storyboards.Drawables;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Graphics.Containers
{
    public class EditorDrawableStoryboard : Container<DrawableStoryboardLayer>
    {
        [Cached]
        public Storyboard Storyboard { get; }

        protected override Container<DrawableStoryboardLayer> Content { get; }

        private readonly IResourceStore<byte[]> resources;
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        public EditorDrawableStoryboard(Storyboard storyboard, IResourceStore<byte[]> resources)
        {
            Storyboard = storyboard;
            RelativeSizeAxes = Axes.Both;

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChild = Content = new Container<DrawableStoryboardLayer>
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };

            this.resources = resources;
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host, EditorClock clock, EditorSessionStatics statics)
        {
            Clock = clock;

            dependencies.CacheAs(new TextureStore(host.CreateTextureLoaderStore(resources), false, scaleAdjust: 1));

            foreach (var layer in Storyboard.Layers)
                Add(layer.CreateDrawable());

            showVideo = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowVideo);
            showBackground = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowBackground);
            showFailing = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowFailing);
            showPassing = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowPassing);
            showForeground = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowForeground);
            showOverlay = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowOverlay);

            bind(showVideo, "Video");
            bind(showBackground, "Background");
            bind(showFailing, "Failing");
            bind(showPassing, "Passing");
            bind(showForeground, "Foreground");
            bind(showOverlay, "Overlay");
        }

        private Bindable<bool> showVideo;
        private Bindable<bool> showBackground;
        private Bindable<bool> showFailing;
        private Bindable<bool> showPassing;
        private Bindable<bool> showForeground;
        private Bindable<bool> showOverlay;

        private void bind(Bindable<bool> bindable, string layerName) => bindable.BindValueChanged(e =>
        {
            var layer = Children.FirstOrDefault(l => l.Name == layerName);

            if (layer == null)
                return;

            layer.Alpha = e.NewValue ? 1 : 0;
        }, true);
    }
}
