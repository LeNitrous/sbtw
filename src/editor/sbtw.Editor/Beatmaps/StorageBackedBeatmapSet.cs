// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets;
using sbtw.Editor.IO;

namespace sbtw.Editor.Beatmaps
{
    public class StorageBackedBeatmapSet
    {
        public BeatmapSetInfo BeatmapSetInfo { get; private set; }

        private readonly RulesetStore rulesets;
        private readonly DemanglingResourceProvider resources;

        public StorageBackedBeatmapSet(DemanglingResourceProvider resources, RulesetStore rulesets)
        {
            this.rulesets = rulesets;
            this.resources = resources;
            Refresh();
        }

        public void Refresh()
        {
            BeatmapSetInfo = new BeatmapSetInfo();

            foreach (string filePath in resources.Storage.GetFiles(string.Empty))
            {
                if (Path.GetExtension(filePath) == ".osu")
                {
                    using var stream = resources.Storage.GetStream(filePath);
                    using var reader = new LineBufferedReader(stream);

                    var beatmap = Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
                    beatmap.BeatmapInfo.Path = filePath;
                    beatmap.BeatmapInfo.Ruleset = rulesets.GetRuleset(beatmap.BeatmapInfo.RulesetID);
                    beatmap.BeatmapInfo.BeatmapSet = BeatmapSetInfo;

                    BeatmapSetInfo.Beatmaps.Add(beatmap.BeatmapInfo);
                    BeatmapSetInfo.Metadata ??= beatmap.Metadata;
                }
            }

            foreach (string filePath in Directory.EnumerateFiles(resources.Storage.GetFullPath("."), "*", SearchOption.AllDirectories))
            {
                BeatmapSetInfo.Files.Add(new BeatmapSetFileInfo
                {
                    Filename = filePath.Replace(resources.Storage.GetFullPath(".") + "\\", string.Empty).Replace("\\", "/"),
                    FileInfo = new osu.Game.IO.FileInfo { Hash = Path.Combine(new string(' ', 2), "$" + filePath.Replace(resources.Storage.GetFullPath(".") + "\\", string.Empty)) }
                });
            }
        }

        public WorkingBeatmap GetWorkingBeatmap(IBeatmapInfo beatmapInfo)
        {
            var ownedBeatmapInfo = BeatmapSetInfo.Beatmaps.FirstOrDefault(b => b == beatmapInfo);

            if (ownedBeatmapInfo == null)
                return null;

            return new StorageBackedWorkingBeatmap(resources, ownedBeatmapInfo);
        }
    }
}
