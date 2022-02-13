// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.Platform;
using osu.Game.Rulesets;
using sbtw.Editor.Beatmaps;

namespace sbtw.Editor.Projects
{
    /// <summary>
    /// Denotes capability of providing beatmaps.
    /// </summary>
    public interface ICanProvideBeatmap
    {
        BeatmapProvider GetBeatmapProvider(GameHost host, AudioManager audioManager, RulesetStore rulesets = null, AudioMixer audioMixer = null);
    }
}
