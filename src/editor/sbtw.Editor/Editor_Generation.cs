// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets.Mods;
using osu.Game.Storyboards;
using sbtw.Editor.Generators;
using sbtw.Editor.Graphics.UserInterface;
using sbtw.Editor.Languages;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor
{
    public partial class Editor
    {
        private CancellationTokenSource generatorTokenSource;

        public void Generate(GenerateKind kind)
        {
            generatorTokenSource?.Cancel();
            generatorTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                bool hasFaulted = false;

                if (kind == GenerateKind.Storyboard)
                {
                    var generator = new EditorStoryboardGenerator(Languages, preview, Beatmap.Value.BeatmapInfo);
                    hasFaulted = await generator.GenerateAsync(Project.Value, Beatmap.Value, generatorTokenSource.Token);
                }

                if (kind == GenerateKind.Osb)
                {
                    if (Project.Value.Groups.Any(g => g.Target.Value == ExportTarget.Storyboard))
                    {
                        var storyboard = new EditorOsbStoryboardGenerator(Languages);
                        hasFaulted = await storyboard.GenerateAsync(Project.Value, Beatmap.Value, generatorTokenSource.Token);
                    }

                    if (Project.Value.Groups.Any(g => g.Target.Value == ExportTarget.Difficulty))
                    {
                        var difficulty = new EditorOsbDifficultyGenerator(Languages);
                        hasFaulted = hasFaulted || await difficulty.GenerateAsync(Project.Value, Beatmap.Value, generatorTokenSource.Token);
                    }
                }

                if (!hasFaulted)
                {
                    var notification = new SimpleErrorNotification
                    {
                        Icon = FontAwesome.Solid.Bomb,
                        Text = @"There are scripts that failed to run. See output for more details.",
                    };

                    notification.Closed += () => output.Show();
                    notifications.Post(notification);
                }
            }, generatorTokenSource.Token).ContinueWith(handleGeneratorFinish);
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

        private abstract class EditorGenerator
        {
            protected readonly LanguageStore Languages;

            protected EditorGenerator(LanguageStore langauges)
            {
                Languages = langauges;
            }

            public abstract Task<bool> GenerateAsync(IProject project, WorkingBeatmap beatmap, CancellationToken token = default);
        }

        private abstract class EditorGenerator<T, U> : EditorGenerator
        {
            protected readonly Logger Logger = Logger.GetLogger("script");

            protected EditorGenerator(LanguageStore langauges)
                : base(langauges)
            {
            }

            protected async Task<GeneratorResult<T, U>> GenerateAsync(IProject project, WorkingBeatmap working, ExportTarget? target = null, CancellationToken token = default)
            {
                Logger.Add($@"Generating storyboard for ""{working}""...");

                var beatmap = working.GetPlayableBeatmap(working.BeatmapInfo.Ruleset, Array.Empty<Mod>(), token);
                var scripts = await Languages.CompileAsync(project.Files, null, token);
                var groups = target == null ? project.Groups : project.Groups.Where(g => g.Target.Value == target);

                var generated = await CreateGenerator().GenerateAsync(new GeneratorConfig
                {
                    Groups = groups,
                    Scripts = scripts,
                    Storage = project.Files,
                    Beatmap = beatmap,
                    Waveform = working.Waveform
                }, token);

                var faulted = generated.Scripts.Where(s => s.Exception != null);
                int exceptionCount = 0;
                if (faulted.Any())
                {
                    foreach (var script in faulted)
                    {
                        var language = Languages.GetLanguageFor(script.Path);
                        if (script.Exception is AggregateException aex)
                        {
                            foreach (var exception in aex.Flatten().InnerExceptions)
                            {
                                Logger.Add(language.GetExceptionMessage(exception), LogLevel.Error);
                                exceptionCount++;
                            }
                        }
                        else
                        {
                            Logger.Add(language.GetExceptionMessage(script.Exception), LogLevel.Error);
                            exceptionCount++;
                        }
                    }
                }

                if (generated.Assets.Any())
                    project.Assets.Generate(generated.Assets);

                var oldGroupNames = project.Groups.Select(g => g.Name);
                var newGroupNames = generated.Groups.Keys;

                var removedGroupNames = oldGroupNames.Except(newGroupNames);
                var addedGroupNames = newGroupNames.Except(oldGroupNames);

                project.Groups.AddRange(addedGroupNames.Select(name => new GroupSetting { Name = name }));
                project.Groups.RemoveAll(group => removedGroupNames.Contains(group.Name));

                project.Scripts.Clear();
                project.Scripts.AddRange(generated.Scripts);

                var message = new StringBuilder();
                message.Append("Generation completed");
                message.AppendLine(faulted.Any() ? $" with {exceptionCount} error(s)." : ".");
                message.AppendLine($"    {generated.Scripts.Count() - faulted.Count()} script(s) loaded.");
                message.AppendLine($"    {generated.Assets.Count()} assets created.");
                Logger.Add(message.ToString());

                return generated;
            }

            protected abstract Generator<T, U> CreateGenerator();
        }

        private class EditorStoryboardGenerator : EditorGenerator<Storyboard, IStoryboardElement>
        {
            private readonly BeatmapInfo beatmapInfo;
            private readonly EditorPreview preview;

            public EditorStoryboardGenerator(LanguageStore languages, EditorPreview preview, BeatmapInfo beatmapInfo)
                : base(languages)
            {
                this.preview = preview;
                this.beatmapInfo = beatmapInfo;
            }

            public override async Task<bool> GenerateAsync(IProject project, WorkingBeatmap beatmap, CancellationToken token = default)
            {
                var generated = await GenerateAsync(project, beatmap, null, token);
                await preview.SetStoryboardAsync(generated.Result, project.Resources.Resources);
                return !generated.Scripts.Any(s => s.Exception != null);
            }

            protected override Generator<Storyboard, IStoryboardElement> CreateGenerator()
                => new StoryboardGenerator(beatmapInfo);
        }

        private abstract class EditorOsbGenerator : EditorGenerator<Dictionary<string, StringBuilder>, StringBuilder>
        {
            public abstract ExportTarget Target { get; }

            protected EditorOsbGenerator(LanguageStore langauges)
                : base(langauges)
            {
            }

            public override sealed async Task<bool> GenerateAsync(IProject project, WorkingBeatmap beatmap, CancellationToken token = default)
            {
                string path = GetTargetFile(beatmap);

                var generated = await GenerateAsync(project, beatmap, Target, token);
                using var stream = project.Resources.Storage.GetStream(path, FileAccess.ReadWrite, FileMode.OpenOrCreate);

                await Perform(generated.Result, stream, token);

                Logger.Add($@"Exported to ""{path}""");

                return !generated.Scripts.Any(s => s.Exception != null);
            }

            protected abstract Task Perform(Dictionary<string, StringBuilder> generated, Stream stream, CancellationToken token = default);

            protected static StringBuilder MakeOsb(Dictionary<string, StringBuilder> generated)
            {
                var builder = new StringBuilder();
                builder.AppendLine("[Events]");
                builder.AppendLine("// Background and Video events");
                builder.Append(generated["Video"]);

                foreach (var layer in Enum.GetValues<Layer>())
                {
                    builder.AppendLine($"// Storyboard Layer {layer} ({Enum.GetName(layer)})");
                    builder.Append(generated[Enum.GetName(layer)]);
                }

                builder.AppendLine("// Storyboard Sound Samples");
                builder.Append(generated["Samples"]);

                return builder;
            }

            protected virtual string GetTargetFile(WorkingBeatmap beatmap)
                => $"{beatmap.Metadata.Artist} - {beatmap.Metadata.Title} ({beatmap.Metadata.AuthorString})";

            protected override Generator<Dictionary<string, StringBuilder>, StringBuilder> CreateGenerator()
                => new OsbGenerator();
        }

        private class EditorOsbStoryboardGenerator : EditorOsbGenerator
        {
            public override ExportTarget Target => ExportTarget.Storyboard;

            public EditorOsbStoryboardGenerator(LanguageStore langauges)
                : base(langauges)
            {
            }

            protected override async Task Perform(Dictionary<string, StringBuilder> generated, Stream stream, CancellationToken token = default)
            {
                using var writer = new StreamWriter(stream);
                stream.Position = 0;
                await writer.WriteAsync(MakeOsb(generated), token);
            }

            protected override string GetTargetFile(WorkingBeatmap beatmap)
                => Path.ChangeExtension(base.GetTargetFile(beatmap), ".osb");
        }

        private class EditorOsbDifficultyGenerator : EditorOsbGenerator
        {
            public override ExportTarget Target => ExportTarget.Difficulty;

            public EditorOsbDifficultyGenerator(LanguageStore langauges)
                : base(langauges)
            {
            }

            protected override async Task Perform(Dictionary<string, StringBuilder> generated, Stream stream, CancellationToken token = default)
            {
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream);

                string contents = await reader.ReadToEndAsync();
                int startIndex = contents.IndexOf("[Events]");
                contents = contents.Remove(startIndex, contents.IndexOf("[TimingPoints]"));
                contents = contents.Insert(startIndex, MakeOsb(generated).ToString());

                stream.Position = 0;
                await writer.WriteAsync(contents);
            }

            protected override string GetTargetFile(WorkingBeatmap beatmap)
                => Path.ChangeExtension(base.GetTargetFile(beatmap) + $" [{beatmap.BeatmapInfo.DifficultyName}]", ".osu");
        }
    }

    public enum GenerateKind
    {
        Osb,
        Storyboard,
    }
}
