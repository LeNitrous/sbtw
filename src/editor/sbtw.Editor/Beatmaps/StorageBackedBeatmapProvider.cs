// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.Platform;
using osu.Game.Rulesets;
using sbtw.Editor.IO.Stores;

namespace sbtw.Editor.Beatmaps
{
    public class StorageBackedBeatmapProvider : BeatmapProvider
    {
        public StorageBackedBeatmapProvider(GameHost host, AudioManager audio, Storage storage, RulesetStore rulesets = null, AudioMixer mixer = null)
            : base(host, audio, new StorageBackedResourceStore(storage), rulesets, mixer)
        {
        }
    }
}
