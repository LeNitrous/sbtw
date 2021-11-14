// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.IO.Compression;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets;
using osu.Game.Screens;
using osu.Game.Screens.Backgrounds;
using osu.Game.Skinning;
using sbtw.Game.Projects;
using sbtw.Game.Screens.Edit;

namespace sbtw.Game.Screens
{
    public class SBTWLoader : OsuScreen
    {
        public override bool AllowBackButton => false;

        protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        [Resolved]
        private Bindable<Project> project { get; set; }

        [Resolved]
        private SkinManager skins { get; set; }

        [Resolved]
        private GameHost host { get; set; }

        [Resolved]
        private AudioManager audio { get; set; }

        [Resolved]
        private RulesetStore rulesets { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            AddRangeInternal(new Drawable[]
            {
                new LoadingSpinner(true)
                {
                    State = { Value = Visibility.Visible },
                }
            });

            project.BindValueChanged(handleProjectChange, true);
            skins.CurrentSkinInfo.Value = skins.DefaultLegacySkin.SkinInfo;
        }

        /// <summary>
        /// Bootstraps a new project provided by a <see cref="ProjectConfiguration"/>
        /// </summary>
        public void CreateProject(ProjectConfiguration config)
        {
            Directory.CreateDirectory(config.Path);

            if (Path.GetExtension(config.BeatmapPath) == ".osz")
            {
                ZipFile.ExtractToDirectory(config.BeatmapPath, Path.Combine(config.Path, "Beatmap"));
            }

            var newProject = new Project(config, host, audio, rulesets);
            newProject.Save();
            newProject.Build();

            project.Value = newProject;
        }

        /// <summary>
        /// Loads an existing project from a given path.
        /// </summary>
        public void LoadProject(string path)
        {
            var loaded = Project.Load(path, host, audio, rulesets);

            if (loaded == null)
            {
                notifications.Post(new SimpleErrorNotification
                {
                    Text = "File is not a SBTW Project.",
                    Icon = FontAwesome.Solid.ExclamationTriangle,
                });
                return;
            }

            project.Value = loaded;
        }

        /// <summary>
        /// Closes the current project.
        /// </summary>
        public void CloseProject()
        {
            ensureCurrent();
            project.Value = null;
            Beatmap.SetDefault();
            pushEditor();
        }

        private ScheduledDelegate scheduledBeatmapChange;

        /// <summary>
        /// Changes the beatmap difficulty with the gi ven<see cref="BeatmapInfo"/>
        /// </summary>
        public void ChangeBeatmap(string version)
        {
            ensureCurrent();

            scheduledBeatmapChange?.Cancel();
            scheduledBeatmapChange = Scheduler.Add(() =>
            {
                Beatmap.Value = project.Value.GetWorkingBeatmap(version);
                Ruleset.Value = Beatmap.Value.BeatmapInfo.Ruleset;
                scheduledBeatmapChange = null;
                pushEditor();
            });
        }

        /// <summary>
        /// Reimports the current beatmap.
        /// </summary>
        public void ReloadBeatmap()
        {
            ChangeBeatmap(Beatmap.Value.BeatmapInfo.Version);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            pushEditor();
        }

        private void ensureCurrent()
        {
            if (this.IsCurrentScreen())
                return;

            ValidForResume = true;
            this.MakeCurrent();
        }

        private void pushEditor()
        {
            this.Push(new SBTWEditor());
            ValidForResume = false;
        }

        private void handleProjectChange(ValueChangedEvent<Project> project)
        {
            if (project.NewValue == null)
                return;

            ensureCurrent();
            ChangeBeatmap(project.NewValue.BeatmapSet.Beatmaps.FirstOrDefault().Version);
        }
    }
}
