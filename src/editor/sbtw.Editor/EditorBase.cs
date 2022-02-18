// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics.Cursor;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Play;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Configuration;
using sbtw.Editor.Generators;
using sbtw.Editor.Generators.Steps;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor
{
    [Cached(typeof(ISamplePlaybackDisabler))]
    public abstract class EditorBase : OsuGameBase, ISamplePlaybackDisabler
    {
        public static readonly Logger ScriptLogger = Logger.GetLogger("script");

        private readonly Bindable<bool> samplePlaybackDisabled = new Bindable<bool>();
        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor(4);

        private DependencyContainer dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        protected override Container<Drawable> Content => content;

        protected readonly EditorSessionStatics Session = new EditorSessionStatics();
        protected readonly BindableList<Group> Groups = new BindableList<Group>();
        protected readonly BindableList<ScriptExecutionResult> Scripts = new BindableList<ScriptExecutionResult>();
        protected readonly Bindable<IProject> Project = new NonNullableBindable<IProject>(new DummyProject());
        protected ScriptManager ScriptManager { get; private set; }
        protected EditorConfigManager EditorConfig { get; private set; }
        protected new Bindable<WorkingBeatmap> Beatmap { get; private set; }
        protected new Bindable<RulesetInfo> Ruleset { get; private set; }
        private readonly EditorBeatmapProvider beatmapProvider = new EditorBeatmapProvider();
        private readonly Container content = new OsuContextMenuContainer { RelativeSizeAxes = Axes.Both };
        private EditorState state;
        private EditorClock clock;
        private Bindable<Group> selected;

        [BackgroundDependencyLoader]
        private void load()
        {
            clock = new EditorClock { IsCoupled = false };
            state = new EditorState(clock);

            Beatmap = base.Beatmap.BeginLease(false);
            Ruleset = base.Ruleset.BeginLease(false);

            dependencies.CacheAs(this);
            dependencies.CacheAs(EditorConfig);
            dependencies.CacheAs(Session);
            dependencies.CacheAs(Project);
            dependencies.CacheAs(Scripts);
            dependencies.CacheAs(Groups);

            dependencies.CacheAs(clock);
            dependencies.CacheAs(beatDivisor);

            clock.SeekingOrStopped.BindTo(samplePlaybackDisabled);

            dependencies.CacheAs<IBeatmapProvider>(beatmapProvider);
            dependencies.CacheAs<IBindable<WorkingBeatmap>>(Beatmap);
            dependencies.CacheAs<IBindable<RulesetInfo>>(Ruleset);
            dependencies.CacheAs<IBindable<IProject>>(Project);

            AddInternal(clock);

            selected = Session.GetBindable<Group>(EditorSessionStatic.GroupSelected);

            base.Content.Add(new PopoverContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = content,
            });

            Ruleset.Value = RulesetStore.AvailableRulesets.FirstOrDefault();
            Project.ValueChanged += handleProjectChange;
        }

        public void SetRuleset(IRulesetInfo rulesetInfo)
        {
            if (rulesetInfo is RulesetInfo concreteRulesetInfo)
                Ruleset.Value = concreteRulesetInfo;

            SetBeatmap(Beatmap.Value.BeatmapInfo);
        }

        public void SetBeatmap(IBeatmapInfo beatmapInfo)
        {
            var beatmap = beatmapProvider.GetBeatmap(beatmapInfo);

            if (beatmap != null)
            {
                var ruleset = Ruleset.Value.CreateInstance();
                var convert = ruleset.CreateBeatmapConverter(beatmap);

                if (!convert.CanConvert() && beatmapInfo.Ruleset is RulesetInfo beatmapRuleset)
                    Ruleset.Value = beatmapRuleset;
            }

            var working = beatmapProvider.GetWorkingBeatmap(beatmapInfo);

            Schedule(() =>
            {
                Beatmap.Value = working;
                clock.Beatmap = beatmapProvider.GetBeatmap(Beatmap.Value.BeatmapInfo);
                clock.ChangeSource(Beatmap.Value.Track);
                state.Apply(working.BeatmapInfo, working.Track.IsRunning);
            });
        }

        public void RefreshBeatmap()
        {
            if (Project.Value is not ICanProvideBeatmap beatmapProvidingProject)
                return;

            int index = beatmapProvider.BeatmapSet.Beatmaps.IndexOf(Beatmap.Value.BeatmapInfo);
            beatmapProvider.Current = beatmapProvidingProject.GetBeatmapProvider(Host, Audio, RulesetStore);
            SetBeatmap(beatmapProvider.Beatmaps[index].BeatmapInfo);
        }

        public async Task<GeneratorResult> Generate(GenerateKind kind, ExportTarget? target = null, bool includeHidden = true, CancellationToken token = default)
        {
            if (ScriptManager == null)
                throw new InvalidOperationException($"{Project.Value.GetType()} is not capable of generating.");

            Schedule(() => OnPreGenerate());

            GeneratorResult output = null;

            var globals = new ScriptGlobals
            {
                Waveform = Beatmap.Value.Waveform,
                Beatmap = beatmapProvider.GetBeatmap(Beatmap.Value.BeatmapInfo),
                Logger = Project.Value as ICanProvideLogger,
                AssetProvider = Project.Value as ICanProvideAssets,
                FileProvider = Project.Value as ICanProvideFiles,
                GroupProvider = Project.Value as ICanProvideGroups,
            };

            try
            {
                switch (kind)
                {
                    case GenerateKind.Storyboard:
                        output = await applyGeneratorConfig(new StoryboardGenerator(ScriptManager), target, includeHidden).GenerateAsync(globals, token);
                        break;

                    default:
                        return null;
                }

                foreach (var exception in output.Scripts.Where(s => s.Faulted).Select(s => s.Exception))
                    ScriptLogger.Add(exception.Message, LogLevel.Error, exception.InnerException);
            }
            catch (Exception e)
            {
                Logger.Error(e, "An error has occured while generating.");
            }

            Schedule(() =>
            {
                Scripts.Clear();

                if (output != null)
                    Scripts.AddRange(output.Scripts);

                OnPostGenerate();
            });

            return output;
        }

        private Generator<T, U> applyGeneratorConfig<T, U>(Generator<T, U> generator, ExportTarget? target, bool includeHidden)
        {
            if (Project.Value is not IGeneratorConfig config)
                return generator;

            if (Project.Value is not ICanProvideAssets assetsProvider)
                throw new InvalidOperationException(@"Project does not provide assets.");

            generator
                .AddStep(new GenerateAssetStep(assetsProvider.BeatmapFiles))
                .AddStep(new FilterGroupStep(target, includeHidden))
                .AddStep(new RoundToPrecisionStep(
                    config.PrecisionMove.Value,
                    config.PrecisionAlpha.Value,
                    config.PrecisionScale.Value,
                    config.PrecisionScale.Value
                ));

            if (config.UseWidescreen.Value)
                generator.AddStep(new OffsetForWidescreenStep());

            return generator;
        }

        protected virtual void OnPreGenerate()
        {
        }

        protected virtual void OnPostGenerate()
        {
        }

        private void handleProjectChange(ValueChangedEvent<IProject> evt)
        {
            Schedule(() =>
            {
                if (evt.OldValue is ICanProvideGroups oldGroupsProvider)
                    Groups.UnbindFrom(oldGroupsProvider.Groups.Bindable);

                if (evt.NewValue is ICanProvideGroups newGroupsProvider)
                    Groups.BindTo(newGroupsProvider.Groups.Bindable);

                Scripts.Clear();
            });

            ScriptManager?.Dispose();
            ScriptManager = null;

            if (Project.Value is ICanProvideFiles fileProvidingProject)
            {
                ScriptManager = new ScriptManager(fileProvidingProject.Files);
            }

            beatmapProvider.Current = null;

            if (Project.Value is ICanProvideBeatmap beatmapProvidingProject)
            {
                beatmapProvider.Current = beatmapProvidingProject.GetBeatmapProvider(Host, Audio, RulesetStore);

                if (beatmapProvider.Beatmaps.Count > 0)
                    SetBeatmap(beatmapProvider.Beatmaps[0].BeatmapInfo);
            }
            else
            {
                clock.Stop();
                clock.ChangeSource(null);
                Beatmap.SetDefault();
            }
        }

        private void handleBeatmapChange()
        {
            selected.Value = null;
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            EditorConfig ??= IsDeployedBuild
                ? new EditorConfigManager(Storage)
                : new DevelopmentEditorConfigManager(Storage);
        }

        protected override void Update()
        {
            base.Update();
            clock.ProcessFrame();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            EditorConfig?.Dispose();
        }

        IBindable<bool> ISamplePlaybackDisabler.SamplePlaybackDisabled => samplePlaybackDisabled;

        private class EditorBeatmapProvider : IBeatmapProvider, IBeatmapResourceProvider
        {
            public IReadOnlyList<IBeatmap> Beatmaps => Current?.Beatmaps ?? Array.Empty<IBeatmap>();
            public IBeatmapSetInfo BeatmapSet => Current?.BeatmapSet ?? new BeatmapSetInfo();
            public IResourceStore<byte[]> Resources => Current?.Resources;
            public IBeatmapProvider Current { get; set; }

            public IBeatmap GetBeatmap(IBeatmapInfo beatmapInfo)
                => Current?.GetBeatmap(beatmapInfo);

            public WorkingBeatmap GetWorkingBeatmap(IBeatmapInfo beatmapInfo)
                => Current?.GetWorkingBeatmap(beatmapInfo);

            TextureStore IBeatmapResourceProvider.LargeTextureStore => (Current as IBeatmapResourceProvider)?.LargeTextureStore;
            ITrackStore IBeatmapResourceProvider.Tracks => (Current as IBeatmapResourceProvider)?.Tracks;
            AudioManager IStorageResourceProvider.AudioManager => (Current as IBeatmapResourceProvider)?.AudioManager;
            IResourceStore<byte[]> IStorageResourceProvider.Files => (Current as IBeatmapResourceProvider)?.Files;
            IResourceStore<byte[]> IStorageResourceProvider.Resources => (Current as IBeatmapResourceProvider)?.Resources;
            RealmAccess IStorageResourceProvider.RealmAccess => (Current as IBeatmapResourceProvider)?.RealmAccess;
            IResourceStore<TextureUpload> IStorageResourceProvider.CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
                => (Current as IBeatmapResourceProvider)?.CreateTextureLoaderStore(underlyingStore);
        }
    }
}
