// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;

namespace sbtw.Editor.Overlays.Setup
{
    public class SetupSectionBeatmap : SetupSection
    {
        public override LocalisableString Title => "Beatmap";

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new LabelledTextBox
                {
                    Label = "Location",
                    RelativeSizeAxes = Axes.X,
                },
                new OsuButton
                {
                    Text = "Browse",
                    Width = 200,
                },
            };
        }
    }
}
