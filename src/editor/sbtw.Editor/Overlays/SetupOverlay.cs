// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osuTK;
using sbtw.Editor.Languages;
using sbtw.Editor.Overlays.Setup;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Overlays
{
    public class SetupOverlay : OsuFocusedOverlayContainer
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        [Cached(name: "ProjectPath")]
        private readonly Bindable<string> projectPath = new Bindable<string>();

        [Cached(name: "ProjectName")]
        private readonly Bindable<string> projectName = new Bindable<string>("my project");

        [Cached(name: "BeatmapPath")]
        private readonly Bindable<string> beatmapPath = new Bindable<string>();

        [Resolved(canBeNull: true)]
        private Editor editor { get; set; }

        [Resolved]
        private ProjectStore projects { get; set; }

        [Resolved]
        private LanguageStore languages { get; set; }

        public SetupOverlay()
        {
            Size = new Vector2(900, 470);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colour)
        {
            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 10,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = colourProvider.Background3,
                        RelativeSizeAxes = Axes.Both,
                    },
                    new SectionsContainer<SetupSection>
                    {
                        Padding = new MarginPadding { Bottom = 60 },
                        RelativeSizeAxes = Axes.Both,
                        FixedHeader = new SetupHeader { Margin = new MarginPadding { Bottom = 5 } },
                        ChildrenEnumerable = new SetupSection[]
                        {
                            new SetupSectionProject(),
                            new SetupSectionBeatmap(),
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                Colour = colourProvider.Dark5,
                                RelativeSizeAxes = Axes.Both,
                            },
                            new OsuButton
                            {
                                Text = "Create",
                                Width = 200,
                                Action = createProject,
                                Margin = new MarginPadding { Vertical = 10, Right = 10 },
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                BackgroundColour = colour.Yellow,
                            },
                        }
                    }
                },
            };
        }

        private void createProject()
        {
            if (editor == null)
                return;

            if (string.IsNullOrEmpty(projectName.Value) &&
                projectPath.Value.IndexOfAny(Path.GetInvalidPathChars()) != -1 &&
                beatmapPath.Value.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                Logger.Log("Failed to create project as there were invalid arguments.", level: LogLevel.Error);
                return;
            }

            var project = projects.Create(projectName.Value, projectPath.Value, languages.Languages.Select(l => l.CreateProjectGenerator()));
            project.Save();

            editor.OpenProject(project);
        }

        protected override void PopIn()
        {
            base.PopIn();
            this
                .ScaleTo(1.0f, 500, Easing.OutBack)
                .FadeIn(500, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            base.PopOut();
            this
                .ScaleTo(0.9f, 500, Easing.OutExpo)
                .FadeOut(500, Easing.OutQuint);
        }
    }
}
