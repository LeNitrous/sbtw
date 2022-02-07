// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Audio.Track;
using osu.Game.Beatmaps;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// Resources that will be sent and used by the script.
    /// </summary>
    public struct ScriptResources
    {
        public IBeatmap Beatmap { get; set; }
        public Waveform Waveform { get; set; }
    }
}
