// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using NUnit.Framework;
using sbtw.Editor.Assets;
using sbtw.Editor.Tests.Projects;

namespace sbtw.Editor.Tests.Assets
{
    public class AssetCollectionTests
    {
        private TemporaryStorageBackedTestProject project;

        [SetUp]
        public void SetUp()
        {
            project = new TemporaryStorageBackedTestProject();
        }

        [TearDown]
        public void TearDown()
        {
            project?.Dispose();
        }

        [Test]
        public void TestAssetCreation()
        {
            var assets = new AssetCollection(project)
            {
                new Rectangle { Path = "a.png" }
            };

            assets.Generate();

            Assert.That(project.BeatmapFiles.Exists("a.png"), Is.True);
            Assert.That(assets, Is.Not.Empty);
        }

        [Test]
        public void TestAssetRenaming()
        {
            var assets = new AssetCollection(project)
            {
                new Rectangle { Path = "a.png" },
                new Rectangle { Path = "b.png" }
            };

            assets.Generate();

            Assert.That(project.BeatmapFiles.Exists("a.png"), Is.False);
            Assert.That(project.BeatmapFiles.Exists("b.png"), Is.True);
            Assert.That(assets, Is.Not.Empty);
        }

        [Test]
        public void TestAssetDeletion()
        {
            var assets = new AssetCollection(project)
            {
                new Rectangle { Path = "a.png" }
            };

            assets.Generate();
            assets.Apply(Enumerable.Empty<Asset>());
            assets.Generate();

            Assert.That(project.BeatmapFiles.Exists("a.png"), Is.False);
            Assert.That(assets, Is.Empty);
        }

        // TODO: Create an asset that doesn't require fonts
        // [Test]
        public void TestAssetUpdating()
        {
            var assets = new AssetCollection(project);
            var a = new Text("hello", new FontConfiguration("a.ttf", "test", 24)) { Path = "a.png" };
            assets.Add(a);

            assets.Generate();

            Assert.That(project.BeatmapFiles.Exists("a.png"), Is.True);
            Assert.That(assets, Is.Not.Empty);

            var b = new Text("goodbye", new FontConfiguration("a.ttf", "test", 24)) { Path = "a.png" };
            assets.Add(b);

            assets.Generate();

            Assert.That(project.BeatmapFiles.Exists("a.png"), Is.True);
            Assert.That(assets, Is.Not.Empty);

            var asset = assets.First() as Text;

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.Path, Is.EqualTo("a.png"));
            Assert.That(asset.DisplayText, Is.EqualTo("goodbye"));
        }
    }
}
