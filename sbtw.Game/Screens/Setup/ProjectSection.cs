// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Screens.Edit.Setup;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Setup
{
    public class ProjectSection : SetupSection
    {
        public override LocalisableString Title => "Project";

        [Resolved]
        private SBTWGame game { get; set; }

        [Resolved]
        private ProjectConfiguration configuration { get; set; }

        private LabelledTextBox path;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new LabelledTextBox
                {
                    Label = "Name",
                    RelativeSizeAxes = Axes.X,
                    Current = configuration.NameBindable,
                },
                path = new LabelledTextBox
                {
                    Label = "Location",
                    RelativeSizeAxes = Axes.X,
                    Current = configuration.PathBindable,
                },
                new OsuButton
                {
                    Text = "Browse",
                    Width = 200,
                    Action = getProjectLocationTask,
                },
            };
        }

        private void getProjectLocationTask()
            => game.SaveFileDialog(configuration.Name, new[] { "*.csproj" }, "MSBuild Project", selected => Schedule(() => path.Text = Path.GetDirectoryName(selected)));
    }
}
