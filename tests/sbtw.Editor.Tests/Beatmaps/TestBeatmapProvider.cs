// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using NUnit.Framework;
using osu.Framework.IO.Stores;
using osu.Game.Beatmaps;
using osu.Game.Resources;
using osu.Game.Tests;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.IO.Storage;

namespace sbtw.Editor.Tests.Beatmaps
{
    public class BeatmapProviderTest : GameHostTest
    {
        [Test]
        public void TestGetBeatmapSet()
        {
            using var store = new DllResourceStore(OsuResources.ResourceAssembly);
            using var host = new CleanRunHeadlessGameHost(callingMethodName: nameof(BeatmapProviderTest));

            try
            {
                var editor = LoadEditor(host);

                using var stream = store.GetStream("Tracks.circles.osz");
                using var storage = new ZipStorage(stream);

                var provider = new StorageBackedBeatmapProvider(host, editor.Audio, storage);
                Assert.That(provider.Beatmaps, Is.Not.Empty);
                Assert.That(provider.BeatmapSet.Beatmaps, Is.Not.Empty);

                var beatmapSet = provider.BeatmapSet as BeatmapSetInfo;
                var resources = provider as IBeatmapResourceProvider;
                Assert.That(resources.Tracks.Get(beatmapSet.GetPathForFile(beatmapSet.Metadata.AudioFile)), Is.Not.Null);

                var beatmap = provider.GetWorkingBeatmap(provider.BeatmapSet.Beatmaps.FirstOrDefault());
                beatmap.LoadTrack();

                Assert.That(beatmap.Waveform, Is.Not.Null);
                Assert.That(beatmap.Track, Is.Not.Null);
            }
            finally
            {
                host.Exit();
            }
        }
    }
}
