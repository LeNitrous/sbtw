// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Skinning;

namespace sbtw.Editor.Beatmaps
{
    public class StorageBackedWorkingBeatmap : WorkingBeatmap
    {
        private readonly IBeatmapResourceProvider resources;
        private readonly IBeatmap beatmap;

        public StorageBackedWorkingBeatmap(IBeatmapResourceProvider resources, IBeatmap beatmap)
            : base(beatmap.BeatmapInfo, resources.AudioManager)
        {
            this.beatmap = beatmap;
            this.resources = resources;
        }

        public override Stream GetStream(string storagePath) => resources.Files.GetStream(storagePath);

        protected override IBeatmap GetBeatmap() => beatmap;

        protected override Texture GetBackground()
        {
            if (string.IsNullOrEmpty(Metadata?.BackgroundFile))
                return null;

            try
            {
                return resources.LargeTextureStore.Get(BeatmapSetInfo.GetPathForFile(Metadata.BackgroundFile));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Background failed to load.");
                return null;
            }
        }

        protected override Track GetBeatmapTrack()
        {
            if (string.IsNullOrEmpty(Metadata?.AudioFile))
                return null;

            try
            {
                return resources.Tracks.Get(BeatmapSetInfo.GetPathForFile(Metadata.AudioFile));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Track failed to load");
                return null;
            }
        }

        protected override ISkin GetSkin()
        {
            try
            {
                return new LegacyBeatmapSkin(BeatmapInfo, resources.Files, resources);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Skin failed to load");
                return null;
            }
        }

        protected override Waveform GetWaveform()
        {
            if (string.IsNullOrEmpty(Metadata?.AudioFile))
                return null;

            try
            {
                var trackData = GetStream(BeatmapSetInfo.GetPathForFile(Metadata.AudioFile));
                return trackData == null ? null : new Waveform(trackData);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Waveform failed to load");
                return null;
            }
        }
    }
}
