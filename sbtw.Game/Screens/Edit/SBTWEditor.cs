// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Input.Bindings;
using osu.Game.Screens;
using osu.Game.Screens.Backgrounds;
using sbtw.Game.Projects;
using sbtw.Game.Screens.Edit.Menus;
using sbtw.Game.Screens.Edit.Setup;

namespace sbtw.Game.Screens.Edit
{
    public class SBTWEditor : SBTWScreen, IKeyBindingHandler<GlobalAction>
    {
        public override float BackgroundParallaxAmount => 0.0f;

        public override bool AllowBackButton => false;

        public override bool DisallowExternalBeatmapRulesetChanges => true;

        public override bool? AllowTrackAdjustments => false;

        protected override BackgroundScreen CreateBackground() => new BackgroundScreenBlack();

        private Container spinner;
        private Container content;
        private TopMenuBar topMenuBar;
        private SetupOverlay setup;

        private EditorContent editor => content.Children.OfType<EditorContent>().FirstOrDefault();

        [Resolved]
        private ProjectManager projectManager { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = content = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                },
                spinner = new LoadingSpinner(true)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                new Container<TopMenuBar>
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 40,
                    Child = topMenuBar = new TopMenuBar
                    {
                        RequestNewProject = () => setup?.Show(),
                        RequestOpenProject = openProject,
                        RequestCloseProject = closeProject,
                        RequestDifficultyChange = openDifficulty,
                        RequestGenerateStoryboard = () => editor?.GenerateStoryboard(),
                    },
                },
                setup = new SetupOverlay(),
            };

            topMenuBar.InterfaceVisibility.BindValueChanged(e =>
            {
                topMenuBar.FadeTo(e.NewValue ? 1 : 0, 200, Easing.OutQuint);

                if (Project.Value is Project)
                    editor?.Controls.FadeTo(e.NewValue ? 1 : 0, 200, Easing.OutQuint);
            });
        }

        private void closeProject()
        {
            if (Project.Value is Project project)
                project.Dispose();

            content.Clear();

            Project.SetDefault();
            Beatmap.SetDefault();

        }

        private void openProject(string path)
        {
            if (Project.Value is Project project)
                project.Dispose();

            Project.Value = projectManager.Load(path);

            var beatmapInfo = Project.Value.BeatmapSet.Beatmaps.FirstOrDefault();

            if (beatmapInfo == null)
                Project.SetDefault();

            openDifficulty(beatmapInfo);
        }

        private void openDifficulty(BeatmapInfo beatmapInfo)
        {
            var working = Project.Value.GetWorkingBeatmap(beatmapInfo.Version);

            if (working == null)
                return;

            // for some reason the track doesn't unload without this!
            Beatmap.SetDefault();

            Beatmap.Value = working;
            Ruleset.Value = beatmapInfo.Ruleset;

            Schedule(spinner.Show);

            LoadComponentAsync(new EditorContent(), loaded =>
            {
                content.Child = loaded;
                spinner.Hide();
            });
        }

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            switch (e.Action)
            {
                case GlobalAction.ToggleInGameInterface:
                    topMenuBar.InterfaceVisibility.Value = !topMenuBar.InterfaceVisibility.Value;
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }
    }
}
