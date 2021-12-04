// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;

namespace sbtw.Game.Screens.Edit.Setup
{
    public class BeatmapSection : SetupSection
    {
        public override LocalisableString Title => "Beatmap";

        public Bindable<string> BeatmapPath
        {
            get => path.Current;
            set => path.Current = value;
        }

        [Resolved]
        private SBTWGame game { get; set; }

        private LabelledTextBox path;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                path = new LabelledTextBox
                {
                    Label = "Location",
                    RelativeSizeAxes = Axes.X,
                },
                new OsuButton
                {
                    Text = "Browse",
                    Width = 200,
                    Action = getBeatmapLocationTask,
                },
            };
        }

        private void getBeatmapLocationTask()
            => game.OpenFileDialog(new[] { "*.osu", "*.osz" }, "Beatmap or Beatmap Archive", selected => Schedule(() => path.Text = selected));
    }
}
