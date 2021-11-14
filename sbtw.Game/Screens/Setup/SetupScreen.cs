// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Screens;
using osu.Game.Screens.Backgrounds;
using osu.Game.Screens.Edit.Setup;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens.Setup
{
    public class SetupScreen : OsuScreen
    {
        public override bool AllowBackButton => true;

        protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

        [Cached]
        protected readonly OverlayColourProvider ColourProvider;

        [Cached]
        private readonly SectionsContainer<SetupSection> sections = new SectionsContainer<SetupSection>();

        [Cached]
        private readonly SetupScreenHeader header = new SetupScreenHeader();

        [Cached]
        private readonly ProjectConfiguration configuration = new ProjectConfiguration();

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        [Resolved]
        private SBTWLoader loader { get; set; }

        public SetupScreen()
        {
            ColourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 250, Vertical = 150 },
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 10,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Colour = ColourProvider.Background3,
                            RelativeSizeAxes = Axes.Both,
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = sections.With(s =>
                            {
                                s.RelativeSizeAxes = Axes.Both;
                                s.FixedHeader = header.With(h => h.Margin = new MarginPadding { Bottom = 5 });
                                s.ChildrenEnumerable = new SetupSection[]
                                {
                                    new BeatmapSection(),
                                    new ProjectSection(),
                                    new ConfirmSection { ConfirmAction = createProject },
                                };
                            }),
                        },
                    }
                }
            };
        }

        private void createProject()
        {
            if (!is_path_valid_beatmap(configuration.BeatmapPath))
            {
                postErrorNotification("Beatmap path is not valid.");
                return;
            }

            if (!is_path_valid(configuration.Path))
            {
                postErrorNotification("Project path is not valid.");
                return;
            }

            if (!File.Exists(configuration.BeatmapPath))
            {
                postErrorNotification("Beatmap file is not found on the given path.");
                return;
            }

            if (Path.GetExtension(configuration.BeatmapPath) == ".osu")
            {
                if (!ProjectHelper.HAS_STABLE)
                {
                    postErrorNotification("No stable installation found. Cannot import beatmap difficulty file.");
                    return;
                }

                if (!configuration.BeatmapPath.Contains(ProjectHelper.STABLE_PATH))
                {
                    postErrorNotification("Beatmap dificulty file must be imported from a stable installation.");
                    return;
                }
                else
                {
                    configuration.BeatmapPath = new DirectoryInfo(configuration.BeatmapPath).Parent.Name;
                    configuration.UseStablePath = true;
                }
            }

            loader.CreateProject(configuration);
        }

        private void postErrorNotification(string reason) => notifications.Post(new SimpleErrorNotification
        {
            Text = reason,
            Icon = FontAwesome.Solid.ExclamationTriangle,
        });

        private static bool is_path_valid(string path)
            => !string.IsNullOrEmpty(path) && Path.IsPathFullyQualified(path);

        private static bool is_path_valid_beatmap(string path)
        {
            bool result = false;

            foreach (string ext in new[] { ".osu", ".osz" })
            {
                result = Path.GetExtension(path) == ext;
                if (result)
                    break;
            }

            return result && is_path_valid(path);
        }
    }
}
