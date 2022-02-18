// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.IO.Compression;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osuTK;
using sbtw.Editor.Overlays.Setup;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

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

        [Resolved]
        private Bindable<IProject> project { get; set; }

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
            if (string.IsNullOrEmpty(projectName.Value) &&
                projectPath.Value.IndexOfAny(Path.GetInvalidPathChars()) != -1 &&
                beatmapPath.Value.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                Logger.Log("Cannot create project as there are invalid arguments.", level: LogLevel.Error);
                return;
            }

            if (Path.GetExtension(beatmapPath.Value) != ".osz")
            {
                Logger.Log("Beatmap path is not a beatmap archive.", level: LogLevel.Error);
                return;
            }

            if (!File.Exists(beatmapPath.Value))
            {
                Logger.Log("Beatmap archive is not found.", level: LogLevel.Error);
                return;
            }

            try
            {
                string fullPath = Path.Combine(projectPath.Value, Path.ChangeExtension(projectName.Value, ".sbtw.json"));
                var project = new JsonBackedProject(fullPath);
                project.Save();

                makeProjectFiles(project);
                this.project.Value = project;
            }
            catch (Exception e)
            {
                Logger.Error(e, "There was a problem during project creation");
            }

            Hide();
        }

        private void makeProjectFiles(JsonBackedProject project)
        {
            foreach (var asm in ScriptManager.Loaded)
            {
                var resources = new NamespacedResourceStore<byte[]>(new DllResourceStore(asm), "Resources/Template");
                foreach (string path in resources.GetAvailableResources())
                {
                    string targetName = path.Replace('_', '.').Replace('@', '.');

                    if (targetName.StartsWith('/'))
                        targetName = targetName.Remove(0);

                    using var target = project.Files.GetStream(targetName, FileAccess.Write);
                    using var stream = resources.GetStream(path);
                    target.Position = 0;
                    stream.CopyTo(target);
                }
            }

            ZipFile.ExtractToDirectory(beatmapPath.Value, project.BeatmapFiles.GetFullPath("."));
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
