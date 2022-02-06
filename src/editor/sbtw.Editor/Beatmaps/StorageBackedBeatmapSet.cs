// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Models;
using osu.Game.Rulesets;
using sbtw.Editor.IO;

namespace sbtw.Editor.Beatmaps
{
    public class StorageBackedBeatmapSet
    {
        public BeatmapSetInfo BeatmapSetInfo { get; private set; }
        public readonly DemanglingResourceProvider Resources;

        private readonly List<Beatmap> beatmaps = new List<Beatmap>();
        private readonly RulesetStore rulesets;

        public StorageBackedBeatmapSet(DemanglingResourceProvider resources, RulesetStore rulesets)
        {
            Resources = resources;
            this.rulesets = rulesets;
            Refresh();
        }

        public void Refresh()
        {
            beatmaps.Clear();
            BeatmapSetInfo = new BeatmapSetInfo();

            foreach (string filePath in Directory.EnumerateFiles(Resources.Storage.GetFullPath("."), "*", SearchOption.AllDirectories))
            {
                string fileName = filePath.Replace(Resources.Storage.GetFullPath(".") + "\\", string.Empty).Replace("\\", "/");
                var file = new RealmFile { Hash = Path.Combine(new string(' ', 2), "$" + filePath.Replace(Resources.Storage.GetFullPath(".") + "\\", string.Empty)) };
                BeatmapSetInfo.Files.Add(new RealmNamedFileUsage(file, fileName));
            }

            foreach (string filePath in Resources.Storage.GetFiles(string.Empty))
            {
                if (Path.GetExtension(filePath) == ".osu")
                {
                    using var stream = Resources.Storage.GetStream(filePath);
                    using var reader = new LineBufferedReader(stream);

                    var beatmap = Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
                    beatmap.BeatmapInfo.BeatmapSet = BeatmapSetInfo;
                    beatmap.BeatmapInfo.Ruleset = rulesets.GetRuleset(beatmap.BeatmapInfo.Ruleset.OnlineID);
                    beatmap.BeatmapInfo.Hash = BeatmapSetInfo.Files.FirstOrDefault(f => f.Filename == Path.GetFileName(filePath)).File.Hash;
                    BeatmapSetInfo.Beatmaps.Add(beatmap.BeatmapInfo);

                    beatmaps.Add(beatmap);
                }
            }
        }

        public WorkingBeatmap GetWorkingBeatmap(IBeatmapInfo beatmapInfo)
        {
            var ownedBeatmap = beatmaps.FirstOrDefault(b => b.BeatmapInfo == beatmapInfo);

            if (ownedBeatmap == null)
                return null;

            return new StorageBackedWorkingBeatmap(Resources, ownedBeatmap);
        }
    }
}
