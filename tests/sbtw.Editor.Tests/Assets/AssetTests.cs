// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using osu.Framework.Testing;
using sbtw.Editor.Assets;

namespace sbtw.Editor.Tests.Assets
{
    public class AssetTests
    {
        private TemporaryNativeStorage storage;

        [SetUp]
        public void SetUp()
        {
            storage = new TemporaryNativeStorage("asset-test");
        }

        [TearDown]
        public void TearDown()
        {
            storage?.Dispose();
        }

        [Test]
        public void TestAssetCreation()
        {
            var assets = new AssetCollection(storage)
            {
                new Rectangle { Path = "a.png" }
            };

            Task.Delay(200).Wait();

            Assert.That(storage.Exists("a.png"), Is.True);
            Assert.That(assets, Is.Not.Empty);
        }

        [Test]
        public void TestAssetRenaming()
        {
            var assets = new AssetCollection(storage)
            {
                new Rectangle { Path = "a.png" },
                new Rectangle { Path = "b.png" }
            };

            Task.Delay(200).Wait();

            Assert.That(storage.Exists("a.png"), Is.False);
            Assert.That(storage.Exists("b.png"), Is.True);
            Assert.That(assets, Is.Not.Empty);
        }

        [Test]
        public void TestAssetDeletion()
        {
            var assets = new AssetCollection(storage)
            {
                new Rectangle { Path = "a.png" }
            };

            Task.Delay(500).Wait();

            assets.Apply(Enumerable.Empty<Asset>());

            Task.Delay(500).Wait();

            Assert.That(storage.Exists("a.png"), Is.False);
            Assert.That(assets, Is.Empty);
        }

        [Test]
        public void TestAssetUpdating()
        {
            var assets = new AssetCollection(storage);
            var a = new Text("hello", new FontConfiguration("a.ttf", "test", 24)) { Path = "a.png" };
            assets.Add(a);

            Task.Delay(200).Wait();

            Assert.That(storage.Exists("a.png"), Is.True);
            Assert.That(assets, Is.Not.Empty);

            var b = new Text("goodbye", new FontConfiguration("a.ttf", "test", 24)) { Path = "a.png" };
            assets.Add(b);

            Task.Delay(200).Wait();

            Assert.That(storage.Exists("a.png"), Is.True);
            Assert.That(assets, Is.Not.Empty);

            var asset = assets.First() as Text;

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.Path, Is.EqualTo("a.png"));
            Assert.That(asset.DisplayText, Is.EqualTo("goodbye"));
        }

        [Test]
        public void TestAssetAdding()
        {

        }
    }
}
