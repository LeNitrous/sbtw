// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Play.PlayerSettings;

namespace sbtw.Game.Screens.Edit.Toolbox
{
    public class LayersToolboxGroup : ToolboxGroup
    {
        public readonly Bindable<bool> Playfield = new Bindable<bool>();
        private readonly Bindable<bool> video = new Bindable<bool>(true);
        private readonly Bindable<bool> background = new Bindable<bool>(true);
        private readonly Bindable<bool> foreground = new Bindable<bool>(true);
        private readonly Bindable<bool> failing = new Bindable<bool>(true);
        private readonly Bindable<bool> passing = new Bindable<bool>(true);
        private readonly Bindable<bool> overlay = new Bindable<bool>(true);

        private readonly Bindable<bool>[] bindables;

        public LayersToolboxGroup()
            : base("Layers")
        {
            Children = new Drawable[]
            {
                new PlayerCheckbox { LabelText = @"Video", Current = video },
                new PlayerCheckbox { LabelText = @"Background", Current = background },
                new PlayerCheckbox { LabelText = @"Foreground", Current = foreground },
                new PlayerCheckbox { LabelText = @"Failing", Current = failing },
                new PlayerCheckbox { LabelText = @"Passing", Current = passing },
                new PlayerCheckbox { LabelText = @"Playfield", Current = Playfield },
                new PlayerCheckbox { LabelText = @"Overlay", Current = overlay },
            };

            bindables = new[]
            {
                video,
                background,
                foreground,
                failing,
                passing,
                overlay
            };
        }

        [BackgroundDependencyLoader]
        private void load(IBindable<EditorDrawableStoryboard> storyboard)
        {
            storyboard.BindValueChanged(handleStoryboardChange, true);
        }

        private void handleStoryboardChange(ValueChangedEvent<EditorDrawableStoryboard> e)
        {
            foreach (var bindable in bindables)
            {
                bindable.Disabled = e.NewValue == null;
                bindable.UnbindEvents();
            }

            if (e.NewValue == null)
                return;

            bind(e.NewValue, video, "Video");
            bind(e.NewValue, background, "Background");
            bind(e.NewValue, foreground, "Foreground");
            bind(e.NewValue, failing, "Failing");
            bind(e.NewValue, passing, "Passing");
            bind(e.NewValue, overlay, "Overlay");
        }

        private static void bind(EditorDrawableStoryboard storyboard, Bindable<bool> bindable, string layerName)
        {
            bindable.BindValueChanged(e =>
            {
                var layer = storyboard.Children.FirstOrDefault(l => l.Layer.Name == layerName);

                if (layer == null)
                    return;

                layer.Alpha = e.NewValue ? 1 : 0;
            }, true);
        }
    }
}
