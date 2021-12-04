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
    public class SBTWLoader : SBTWScreen
    {
        public override bool AllowBackButton => false;

        protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

        [Resolved]
        private NotificationOverlay notifications { get; set; }

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

            Project.BindValueChanged(handleProjectChange, true);
            skins.CurrentSkinInfo.Value = skins.DefaultLegacySkin.SkinInfo;
        }

        /// <summary>
        /// Closes the current project.
        /// </summary>
        public void CloseProject()
        {
            ensureCurrent();
            Project.SetDefault();
            pushEditor();
        }

        private ScheduledDelegate scheduledBeatmapChange;

        /// <summary>
        /// Changes the beatmap difficulty with the specified verison name.
        /// </summary>
        public void ChangeBeatmap(string version)
            => ChangeBeatmap(Project.Value.GetWorkingBeatmap(version));

        /// <summary>
        /// Changes the beatmap difficulty with the specified <see cref="WorkingBeatmap"/>
        /// </summary>
        public void ChangeBeatmap(WorkingBeatmap beatmap)
        {
            ensureCurrent();

            scheduledBeatmapChange?.Cancel();
            scheduledBeatmapChange = Scheduler.Add(() =>
            {
                Beatmap.Value = beatmap;
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

        private void handleProjectChange(ValueChangedEvent<IProject> project)
        {
            ensureCurrent();

            if (project.NewValue is DummyProject)
            {
                Beatmap.SetDefault();
            }

            if (project.NewValue is Project loadable)
            {
                ChangeBeatmap(loadable.BeatmapSet.Beatmaps.FirstOrDefault().Version);
            }
        }
    }
}
