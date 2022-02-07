// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Storyboards;
using sbtw.Editor.Assets;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Generators;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Projects
{
    public abstract class Project : IProject, ICanProvideAssets, ICanProvideGroups, ICanProvideScripts, ICanProvideFiles, ICanProvideLogger, ICanProvideBeatmapSet
    {
        public string Name { get; }
        public string Path => Files.GetFullPath(".");
        public IEnumerable<string> Exclude { get; }
        public Storage Files { get; }
        public ScriptManager Scripts { get; }
        public GroupCollection Groups { get; }
        public AssetCollection Assets { get; }
        public BeatmapSetProvider BeatmapSet { get; }

        protected Project(string path, IEnumerable<Type> types)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Files = CreateStorage(path);
            Groups = new GroupCollection();
            Assets = new AssetCollection(Files);
            Scripts = new ScriptManager(this, types);
            BeatmapSet = CreateBeatmapSetProvider();
        }

        public Storyboard GetStoryboard() => GetStoryboardAsync().Result;

        public async Task<Storyboard> GetStoryboardAsync(CancellationToken token = default)
        {
            var working = BeatmapSet.GetWorkingBeatmap();
            var resources = new ScriptResources { Beatmap = working.Beatmap, Waveform = working.Waveform };
            var generator = new StoryboardGenerator(working.BeatmapInfo as BeatmapInfo);
            var scripts = await Scripts.GetScriptsAsync(resources, token);
            return await generator.GenerateAsync(scripts, this, token);
        }

        public StringBuilder GetOsb() => GetOsbAsync().Result;

        public async Task<StringBuilder> GetOsbAsync(CancellationToken token = default)
        {
            var working = BeatmapSet.GetWorkingBeatmap();
            var resources = new ScriptResources { Beatmap = working.Beatmap, Waveform = working.Waveform };
            var generator = new OsbGenerator();
            var scripts = await Scripts.GetScriptsAsync(resources, token);
            var generated = await generator.GenerateAsync(scripts, this, token);
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

        public abstract void Log(object message, LogLevel level);
        protected abstract Storage CreateStorage(string path);
        protected abstract BeatmapSetProvider CreateBeatmapSetProvider();
    }
}
