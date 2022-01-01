// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using osu.Framework.IO.Stores;
using osu.Game.Resources;
using osu.Game.Tests;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.IO;

namespace sbtw.Editor.Tests.Beatmaps
{
    public class StorageBackedBeatmapSetTest : GameHostTest
    {
        [Test]
        public void TestPopulatedStorage()
        {
            using var store = new DllResourceStore(OsuResources.ResourceAssembly);
            using var host = new CleanRunHeadlessGameHost(callingMethodName: nameof(StorageBackedBeatmapSetTest));

            try
            {
                var editor = LoadEditor(host);

                using (var stream = store.GetStream("Tracks.circles.osz"))
                {
                    using var writer = host.Storage.GetStream("circles.osz", FileAccess.Write);
                    stream.CopyTo(writer);
                }

                ZipFile.ExtractToDirectory(host.Storage.GetFullPath("./circles.osz"), host.Storage.GetFullPath("."));
                host.Storage.Delete("circles.osz");

                var beatmapSet = new StorageBackedBeatmapSet(new DemanglingResourceProvider(host, editor.Audio, host.Storage), editor.RulesetStore);
                Assert.That(beatmapSet.BeatmapSetInfo.Beatmaps, Is.Not.Empty);

                var beatmap = beatmapSet.GetWorkingBeatmap(beatmapSet.BeatmapSetInfo.Beatmaps.FirstOrDefault());
                Assert.That(beatmap, Is.Not.Null);
                Assert.That(beatmap.Skin, Is.Not.Null);

                beatmap.LoadTrack();

                Assert.That(beatmap.Track, Is.Not.Null);
                Assert.That(beatmap.Beatmap, Is.Not.Null);

                // TODO: Provide own test resources
                // Assert.That(beatmap.Background, Is.Not.Null);
            }
            finally
            {
                host.Exit();
            }
        }

        [Test]
        public void TestUnpopulatedStorage()
        {
            using var host = new CleanRunHeadlessGameHost(callingMethodName: nameof(StorageBackedBeatmapSetTest));

            try
            {
                var editor = LoadEditor(host);
                var beatmapSet = new StorageBackedBeatmapSet(new DemanglingResourceProvider(host, editor.Audio, host.Storage), editor.RulesetStore);
                Assert.That(beatmapSet.BeatmapSetInfo.Beatmaps, Is.Empty);
            }
            finally
            {
                host.Exit();
            }
        }
    }
}
