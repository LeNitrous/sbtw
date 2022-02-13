// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays.Settings;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public class LayersToolbox : EditorToolbox
    {
        public LayersToolbox()
            : base("Layers")
        {
        }

        [BackgroundDependencyLoader]
        private void load(EditorSessionStatics statics)
        {
            Children = new Drawable[]
            {
                new SettingsCheckbox { ShowsDefaultIndicator = false, LabelText = @"Video", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowVideo), },
                new SettingsCheckbox { ShowsDefaultIndicator = false, LabelText = @"Background", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowBackground) },
                new SettingsCheckbox { ShowsDefaultIndicator = false, LabelText = @"Failing", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowFailing)},
                new SettingsCheckbox { ShowsDefaultIndicator = false, LabelText = @"Passing", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowPassing) },
                new SettingsCheckbox { ShowsDefaultIndicator = false, LabelText = @"Foreground", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowForeground) },
                new SettingsCheckbox { ShowsDefaultIndicator = false, LabelText = @"Playfield", Current = statics.GetBindable<bool>(EditorSessionStatic.ShowPlayfield) },
                new SettingsCheckbox { ShowsDefaultIndicator = false, LabelText = @"Overlay", Current = statics.GetBindable<bool>(EditorSessionStatic.StoryboardShowOverlay) },
            };
        }
    }
}
