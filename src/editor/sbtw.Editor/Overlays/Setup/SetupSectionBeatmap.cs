// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;

namespace sbtw.Editor.Overlays.Setup
{
    public class SetupSectionBeatmap : SetupSection
    {
        public override LocalisableString Title => "Beatmap";

        [Resolved(name: "BeatmapPath")]
        private Bindable<string> beatmapPath { get; set; }

        [Resolved]
        private EditorBase editor { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new LabelledTextBox
                {
                    Label = "Location",
                    Current = beatmapPath,
                    RelativeSizeAxes = Axes.X,
                },
                new OsuButton
                {
                    Text = "Browse",
                    Width = 200,
                    Action = () => Task.Run(async () => beatmapPath.Value = await editor.RequestSingleFileAsync("Open beatmap", null, extensions: new[] { ".osz" })),
                },
            };
        }
    }
}
