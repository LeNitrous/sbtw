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
    public class SetupSectionProject : SetupSection
    {
        public override LocalisableString Title => "Project";

        [Resolved(name: "ProjectPath")]
        private Bindable<string> projectPath { get; set; }

        [Resolved(name: "ProjectName")]
        private Bindable<string> projectName { get; set; }

        [Resolved]
        private EditorBase editor { get; set; }


        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new LabelledTextBox
                {
                    Label = "Name",
                    Current = projectName,
                    RelativeSizeAxes = Axes.X,
                },
                new LabelledTextBox
                {
                    Label = "Location",
                    Current = projectPath,
                    RelativeSizeAxes = Axes.X,
                },
                new OsuButton
                {
                    Text = "Browse",
                    Width = 200,
                    Action = browse,
                },
            };
        }

        private void browse() => Task.Run(async () =>
        {
            string path = await editor.RequestPathAsync("Select project path");
            Schedule(() => projectPath.Value = path);
        });
    }
}
