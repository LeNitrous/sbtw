// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Edit.Setup
{
    public class ProjectSection : SetupSection
    {
        public override LocalisableString Title => "Project";

        public Bindable<string> ProjectName
        {
            get => name.Current;
            set => name.Current = value;
        }

        public Bindable<string> ProjectPath
        {
            get => path.Current;
            set => path.Current = value;
        }

        public Bindable<ProjectTemplateType> Template
        {
            get => template.Current;
            set => template.Current = value;
        }

        [Resolved]
        private SBTWGame game { get; set; }

        private LabelledTextBox path;
        private LabelledTextBox name;
        private LabelledDropdown<ProjectTemplateType> template;


        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                template = new LabelledDropdown<ProjectTemplateType>
                {
                    Label = "Template",
                    Items = Enum.GetValues<ProjectTemplateType>(),
                },
                name = new LabelledTextBox
                {
                    Label = "Name",
                    RelativeSizeAxes = Axes.X,
                },
                path = new LabelledTextBox
                {
                    Label = "Location",
                    RelativeSizeAxes = Axes.X,
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
            => game.SaveFileDialog(name.Current.Value, new[] { "*.csproj" }, "MSBuild Project", selected => Schedule(() => path.Text = Path.GetDirectoryName(selected)));
    }
}
