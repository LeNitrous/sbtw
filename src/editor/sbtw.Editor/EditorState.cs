// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Beatmaps;
using osu.Game.Screens.Edit;

namespace sbtw.Editor
{
    public class EditorState
    {
        public IBeatmapInfo BeatmapInfo { get; private set; } = new BeatmapInfo();
        private readonly EditorClock clock;

        public EditorState(EditorClock clock)
        {
            this.clock = clock;
        }

        public void Apply(IBeatmapInfo beatmapInfo, bool playing)
        {
            if (beatmapInfo.Metadata.Title != BeatmapInfo.Metadata.Title)
            {
                if (playing)
                    clock.Stop();

                clock.Seek(0);
            }

            BeatmapInfo = beatmapInfo;
        }
    }
}
