// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using sbtw.Editor.Platform;

namespace sbtw.Editor.Overlays.Setup
{
    public class SetupSectionBeatmap : SetupSection
    {
        public override LocalisableString Title => "Beatmap";

        [Resolved(name: "BeatmapPath")]
        private Bindable<string> beatmapPath { get; set; }

        [Resolved]
        private EditorBase editor { get; set; }

        private LabelledTextBox location;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                location = new LabelledTextBox
                {
                    Label = "Location",
                    Current = beatmapPath,
                    RelativeSizeAxes = Axes.X,
                },
                new OsuButton
                {
                    Text = "Browse",
                    Width = 200,
                    Action = () => Task.Run(browseBeatmapArchive),
                },
            };
        }

        private async Task browseBeatmapArchive()
        {
            if (editor is not DesktopEditor desktopEditor)
                return;

            var filter = new PickerFilter { Files = new[] { "*.osz" }, Description = "osu! Beatmap Archive" };
            string result = (await desktopEditor.Picker.OpenFileAsync(new[] { filter })).FirstOrDefault();

            if (string.IsNullOrEmpty(result))
                return;

            Schedule(() => location.Text = result);
        }
    }
}
