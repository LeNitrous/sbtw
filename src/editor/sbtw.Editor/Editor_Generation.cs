// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets.Mods;
using sbtw.Editor.Generators;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor
{
    public partial class Editor
    {
        [Cached(typeof(IBindableList<ScriptGenerationResult>))]
        private readonly BindableList<ScriptGenerationResult> scripts = new BindableList<ScriptGenerationResult>();

        private CancellationTokenSource generatorTokenSource;

        public void Generate(GenerateKind kind)
        {
            if (kind == GenerateKind.Osb)
            {
                generate(async () =>
                {
                    var difficulty = await generate(new OsbGenerator(), GenerateTarget.Difficulty, true, generatorTokenSource.Token);
                    var storyboard = await generate(new OsbGenerator(), GenerateTarget.Storyboard, true, generatorTokenSource.Token);
                    string file = $"{Beatmap.Value.Metadata.Artist} - {Beatmap.Value.Metadata.Title} ({Beatmap.Value.Metadata.AuthorString})";

                    {
                        using var stream = Project.Value.Resources.Storage.GetStream($"{file}.osb", FileAccess.Write);
                        using var writer = new StreamWriter(stream);
                        stream.Position = 0;
                        await writer.WriteAsync(storyboard.Result.ToString());
                    }

                    {
                        using var stream = Project.Value.Resources.Storage.GetStream($"{file} [{Beatmap.Value.BeatmapInfo.DifficultyName}].osu", FileAccess.ReadWrite, FileMode.Open);
                        using var reader = new StreamReader(stream);
                        using var writer = new StreamWriter(stream);

                        string diff = await reader.ReadToEndAsync();
                        stream.Position = diff.IndexOf("[Events]");
                        await writer.WriteAsync(difficulty.Result.ToString());
                    }
                });
            }
            else if (kind == GenerateKind.Storyboard)
            {
                generate(async () =>
                {
                    var generated = await generate(new StoryboardGenerator(Beatmap.Value.BeatmapInfo), GenerateTarget.All, false, generatorTokenSource.Token);
                    Schedule(() => preview.SetStoryboard(generated.Result, Project.Value.Resources.Resources));
                });
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        private void generate(Action action)
        {
            generatorTokenSource?.Cancel();

            if (Project.Value is DummyProject || Beatmap.Value is DummyWorkingBeatmap && (preview?.IsLoaded ?? false))
                return;

            generatorTokenSource = new CancellationTokenSource();

            Schedule(() => spinner.Show());
            Task.Run(action, generatorTokenSource.Token).ContinueWith(handleGeneratorFinish);
        }

        private async Task<GeneratorResult<T, U>> generate<T, U>(Generator<T, U> generator, GenerateTarget target, bool excludeNonVisible, CancellationToken token)
        {
            Schedule(() => output.Clear());
            logger.Add($@"Generating storyboard for ""{Beatmap.Value}""...");

            IEnumerable<ElementGroupSetting> groups = Project.Value.Groups;

            if (target == GenerateTarget.Difficulty)
                groups = groups.Where(g => g.ExportToDifficulty.Value);

            if (target == GenerateTarget.Storyboard)
                groups = groups.Where(g => !g.ExportToDifficulty.Value);

            if (excludeNonVisible)
                groups = groups.Where(g => g.Visible.Value);

            var compiled = await Languages.CompileAsync(Project.Value.Files, null, token);

            var generated = await generator.GenerateAsync(new GeneratorConfig
            {
                Scripts = compiled,
                Storage = Project.Value.Files,
                Beatmap = Beatmap.Value.GetPlayableBeatmap(Beatmap.Value.BeatmapInfo.Ruleset, new List<Mod>(), token),
                Waveform = Beatmap.Value.Waveform,
                Ordering = groups.Select(g => g.Name),
            }, token);

            int loadedCount = generated.Scripts.Where(s => !s.Faulted).Count();
            int assetsCount = generated.Assets.Count();
            int faultedCount = generated.Scripts.Where(s => s.Faulted).Count();

            if (generated.Assets.Any())
                Project.Value.Assets.Generate(generated.Assets);

            if (faultedCount > 0)
            {
                var notification = new SimpleErrorNotification
                {
                    Icon = FontAwesome.Solid.Bomb,
                    Text = @"There are scripts that failed to run. See output for more details.",
                };

                notification.Closed += () => output.Show();
                notifications.Post(notification);
            }

            Schedule(() =>
            {
                Project.Value.Groups.Clear();
                Project.Value.Groups.AddRange(generated.Groups.Select(g => new ElementGroupSetting { Name = g }));

                scripts.Clear();
                scripts.AddRange(generated.Scripts);

                string withFaultedMessage = faultedCount > 0 ? $" with {faultedCount} errors." : ".";

                logger.Add($@"Generation completed{withFaultedMessage}
    {loadedCount} script(s) loaded.
    {assetsCount} asset(s) generated.");
            });

            return generated;
        }

        private void handleGeneratorFinish(Task task)
        {
            Schedule(() => spinner.Hide());

            if (task.Exception == null)
                return;

            switch (task.Exception.InnerException)
            {
                case UnauthorizedAccessException uae:
                    Logger.Error(uae, "Failed to generate storyboard due to lack of permissions.");
                    break;

                case Exception ex:
                    Logger.Error(ex, "An unknown error has occured while attempting to generate.");
                    break;
            }
        }

        private enum GenerateTarget
        {
            All,
            Difficulty,
            Storyboard,
        }
    }

    public enum GenerateKind
    {
        Osb,
        Storyboard,
    }
}
