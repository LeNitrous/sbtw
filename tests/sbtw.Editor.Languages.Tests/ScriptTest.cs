// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Testing;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Languages.Tests
{
    public abstract class ScriptTest
    {
        protected TemporaryNativeStorage Storage { get; private set; }

        protected abstract string Extension { get; }

        protected abstract Stream GetStream(string path);
        protected abstract Script CreateScript(string path);

        [OneTimeSetUp]
        public void SetUp()
        {
            Storage = new TemporaryNativeStorage(GetType().Assembly.GetName().Name);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Storage?.Dispose();
        }

        [Test]
        public void TestGroupCreation()
        {
            var script = Generate("groupCreation");
            Assert.That(script.Groups, Is.Not.Empty);
            Assert.That(script.Groups.Any(g => g.Name == "TestGroup"), Is.True);
        }

        [Test]
        public void TestVideoCreation()
        {
            var script = Generate("videoCreation");
            Assert.That(script.Groups, Is.Not.Empty);
            Assert.That(script.Groups.Any(g => g.Name == "Video"), Is.True);
            Assert.That(script.Groups.FirstOrDefault(g => g.Name == "Video").Elements.OfType<ScriptedVideo>(), Is.Not.Empty);

            var video = script.Groups.FirstOrDefault(g => g.Name == "Video").Elements.OfType<ScriptedVideo>().FirstOrDefault();
            Assert.That(video.Path, Is.EqualTo("test.mp4"));
            Assert.That(video.StartTime, Is.EqualTo(1000));
        }

        public ScriptGenerationResult Generate(string name)
        {
            string filename = $"{name}.{Extension}";
            return CreateScript(Copy(filename)).Generate();
        }

        public string Copy(string file)
        {
            using var rStream = GetStream(file);
            using var wStream = Storage.GetStream(file, FileAccess.Write);
            wStream.Position = 0;
            rStream.CopyTo(wStream);
            return Storage.GetFullPath(file);
        }
    }
}
