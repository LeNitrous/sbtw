// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osuTK;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Edit.Setup
{
    public class SetupOverlay : OsuFocusedOverlayContainer
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider;

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        [Resolved]
        private ProjectManager projectManager { get; set; }

        [Resolved]
        private SBTWEditor editor { get; set; }

        private BeatmapSection beatmapSection;
        private ProjectSection projectSection;

        protected override bool StartHidden => true;

        public SetupOverlay()
        {
            colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);
            Size = new Vector2(900, 550);
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
                        FixedHeader = new SetupScreenHeader { Margin = new MarginPadding { Bottom = 5 } },
                        ChildrenEnumerable = new SetupSection[]
                        {
                            beatmapSection = new BeatmapSection(),
                            projectSection = new ProjectSection(),
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
                                Margin = new MarginPadding { Vertical = 10, Right = 10 },
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                BackgroundColour = colour.Yellow,
                                Action = createProject,
                            },
                        }
                    }
                },
            };
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

        private void createProject()
        {
            string projectName = projectSection.ProjectName.Value;
            string projectPath = projectSection.ProjectPath.Value;
            string beatmapPath = beatmapSection.BeatmapPath.Value;
            var template = projectSection.Template.Value;

            IProject project = null;
            try
            {
                project = projectManager?.Create(projectName, projectPath, beatmapPath, template);
            }
            catch (InvalidBeatmapPathException e)
            {
                postErrorNotification(e, "Beatmap path is not valid.");
            }
            catch (FileNotFoundException e)
            {
                postErrorNotification(e, "Beatmap file is not found on the given path.");
            }
            catch (PlatformNotSupportedException e)
            {
                postErrorNotification(e, e.Message);
            }
            catch (InvalidOperationException e)
            {
                postErrorNotification(e, "Beatmap dificulty file must be imported from a stable installation.");
            }
            catch (InvalidProjectPathException e)
            {
                postErrorNotification(e, "Project path is not valid.");
            }
            catch (NonEmptyProjectPathException e)
            {
                postErrorNotification(e, "Project path must be an empty directory.");
            }
            catch (Exception e)
            {
                postErrorNotification(e, "Failed to create project. This may be due to the operating system or antivirus' protection rules.");
            }

            if (project == null)
                return;

            project.Save();

            Hide();
            editor.OpenProject(project);
        }

        private void postErrorNotification(Exception e, string reason)
        {
            Logger.Error(e, reason);
            notifications.Post(new SimpleErrorNotification
            {
                Text = reason,
                Icon = FontAwesome.Solid.ExclamationTriangle,
            });
        }
    }
}
