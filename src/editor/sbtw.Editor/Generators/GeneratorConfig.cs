// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Audio.Track;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Graphics;

namespace sbtw.Editor.Generators
{
    public struct GeneratorConfig
    {
        public Storage Storage { get; set; }
        public IBeatmap Beatmap { get; set; }
        public Waveform Waveform { get; set; }
        public IEnumerable<Asset> Assets { get; set; }
        public IEnumerable<Script> Scripts { get; set; }
        public IEnumerable<string> Ordering { get; set; }
        public IReadOnlyDictionary<string, IEnumerable<ScriptVariableInfo>> Variables { get; set; }
    }
}
