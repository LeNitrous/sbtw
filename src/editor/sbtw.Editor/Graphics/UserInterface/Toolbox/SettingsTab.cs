// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays.Settings;
using osuTK;
using sbtw.Editor.Generators;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public class SettingsTab : EditorTabbedToolboxTab
    {
        protected override Container<Drawable> Content { get; }

        private IBindable<IProject> project;

        public SettingsTab()
        {
            InternalChild = new OsuScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = true,
                Child = Content = new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Top = 10 },
                    Spacing = new Vector2(0, 10),
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(IBindable<IProject> project)
        {
            this.project = project.GetBoundCopy();
            this.project.ValueChanged += e => Schedule(() => handleProjectChange(e.NewValue));
        }

        private void handleProjectChange(IProject project)
        {
            Clear();

            if (project is not IGeneratorConfig config)
                return;


            AddRange(new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = @"Use Widescreen",
                    TooltipText = @"When true, the generator will offset all sprites and animations to the widescreen's top-left corner.",
                    Current = config.UseWidescreen.GetBoundCopy(),
                },
                new SettingsSlider<int>
                {
                    LabelText = @"Move Precision",
                    TooltipText = @"The number of decimals used for move-related commands.",
                    Current = config.PrecisionMove.GetBoundCopy(),
                },
                new SettingsSlider<int>
                {
                    LabelText = @"Scale Precision",
                    TooltipText = @"The number of decimals used for scale-related commands.",
                    Current = config.PrecisionScale.GetBoundCopy(),
                },
                new SettingsSlider<int>
                {
                    LabelText = @"Alpha Precision",
                    TooltipText = @"The number of decimals used for alpha-related commands.",
                    Current = config.PrecisionAlpha.GetBoundCopy(),
                },
                new SettingsSlider<int>
                {
                    LabelText = @"Rotation Precision",
                    TooltipText = @"The number of decimals used for rotation-related commands.",
                    Current = config.PrecisionRotation.GetBoundCopy(),
                },
            });
        }
    }
}
