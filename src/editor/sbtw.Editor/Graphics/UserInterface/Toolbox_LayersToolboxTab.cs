// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Screens.Play.PlayerSettings;
using osuTK;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Graphics.UserInterface
{
    public partial class Toolbox
    {
        private class LayersToolboxTab : FillFlowContainer
        {
            [BackgroundDependencyLoader]
            private void load(EditorSessionStatics statics)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Direction = FillDirection.Vertical;
                Spacing = new Vector2(0, 5);
                Children = new Drawable[]
                {
                    new PlayerCheckbox { LabelText = @"Video", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowVideo) },
                    new PlayerCheckbox { LabelText = @"Background", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowBackground) },
                    new PlayerCheckbox { LabelText = @"Failing", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowFailing)},
                    new PlayerCheckbox { LabelText = @"Passing", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowPassing) },
                    new PlayerCheckbox { LabelText = @"Foreground", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowForeground) },
                    new PlayerCheckbox { LabelText = @"Playfield", Current = statics.GetBindable<bool>(EditorSessionStatic.ShowPlayfield) },
                    new PlayerCheckbox { LabelText = @"Overlay", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowOverlay) },
                };
            }
        }
    }
}
