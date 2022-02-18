// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using NUnit.Framework;
using osu.Framework.Testing;

namespace sbtw.Editor.Tests.Scripts
{
    public class FileBasedScriptLanguageTests
    {
        private TestFileBasedScriptLanguage lang;
        private TemporaryNativeStorage storage;

        [SetUp]
        public void SetUp()
        {
            storage = new TemporaryNativeStorage(nameof(FileBasedScriptLanguageTests));
            lang = new TestFileBasedScriptLanguage(storage);
            writeToStorage("file one", "Hello World");
            writeToStorage("file two", "Lorem Ipsum");
        }

        [TearDown]
        public void TearDown()
        {
            lang?.Dispose();
            storage?.Dispose();
        }

        private void writeToStorage(string name, string text)
        {
            using var stream = storage.GetStream($"{name}.txt", FileAccess.Write);
            using var writer = new StreamWriter(stream);
            stream.Position = 0;
            writer.Write(text);
        }

        [Test]
        public void TestScriptManagerGetScripts()
        {
            var scripts = lang.Execute<object>(null);
            Assert.That(scripts, Is.Not.Empty);
            Assert.That(lang.Cache, Is.Not.Empty);
        }

        [Test]
        public void TestScriptManagerScriptCompiling()
        {
            lang.Execute<object>(null);

            var cache = lang.Cache;
            Assert.That(cache[0].Compiled, Is.EqualTo("Hello World"));
            Assert.That(cache[1].Compiled, Is.EqualTo("Lorem Ipsum"));
        }

        [Test]
        public void TestScriptManagerScriptRecompiling()
        {
            lang.Execute<object>(null);

            var cache = lang.Cache;

            Assert.That(cache[0].CompileCount, Is.EqualTo(1));
            Assert.That(cache[1].CompileCount, Is.EqualTo(1));
            Assert.That(cache[0].Compiled, Is.EqualTo("Hello World"));
            Assert.That(lang.Cache.Count, Is.EqualTo(2));

            writeToStorage("file one", "Goodbye World");
            lang.Execute<object>(null);

            Assert.That(cache[0].CompileCount, Is.EqualTo(2));
            Assert.That(cache[1].CompileCount, Is.EqualTo(1));
            Assert.That(cache[0].Compiled, Is.EqualTo("Goodbye World"));
            Assert.That(lang.Cache.Count, Is.EqualTo(2));
        }
    }
}
