// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using sbtw.Editor.Tests.Projects;

namespace sbtw.Editor.Tests.Scripts
{
    public class FileBasedScriptLanguageTests
    {
        private FileBasedScriptLanguageTestProject project;

        [SetUp]
        public void SetUp()
        {
            project = new FileBasedScriptLanguageTestProject();
            writeToStorage("a", "Hello World");
            writeToStorage("b", "Lorem Ipsum");
        }

        [TearDown]
        public void TearDown()
        {
            project?.Dispose();
        }

        private void writeToStorage(string name, string text)
        {
            using var stream = project.Files.GetStream($"{name}.txt", FileAccess.Write);
            using var writer = new StreamWriter(stream);
            stream.Position = 0;
            writer.Write(text);
        }

        [Test]
        public void TestScriptManagerGetScripts()
        {
            var scripts = project.Scripts.GetScripts();
            Assert.That(scripts, Is.Not.Empty);
            Assert.That((project.Language as TestFileBasedScriptLanguage).Cache, Is.Not.Empty);
        }

        [Test]
        public void TestScriptManagerScriptCompiling()
        {
            var scripts = project.Scripts.GetScripts().ToArray();
            Assert.That((scripts[0] as TestFileBasedScript).Compiled, Is.EqualTo("Hello World"));
            Assert.That((scripts[1] as TestFileBasedScript).Compiled, Is.EqualTo("Lorem Ipsum"));
        }

        [Test]
        public void TestScriptManagerScriptRecompiling()
        {
            var scripts = project.Scripts.GetScripts().ToArray();
            Assert.That((scripts[0] as TestFileBasedScript).CompileCount, Is.EqualTo(1));
            Assert.That((scripts[1] as TestFileBasedScript).CompileCount, Is.EqualTo(1));
            Assert.That((scripts[0] as TestFileBasedScript).Compiled, Is.EqualTo("Hello World"));
            Assert.That((project.Language as TestFileBasedScriptLanguage).Cache.Count, Is.EqualTo(2));

            writeToStorage("a", "Goodbye World");
            scripts = project.Scripts.GetScripts().ToArray();

            Assert.That((scripts[0] as TestFileBasedScript).CompileCount, Is.EqualTo(2));
            Assert.That((scripts[1] as TestFileBasedScript).CompileCount, Is.EqualTo(1));
            Assert.That((scripts[0] as TestFileBasedScript).Compiled, Is.EqualTo("Goodbye World"));
            Assert.That((project.Language as TestFileBasedScriptLanguage).Cache.Count, Is.EqualTo(2));
        }

        private class FileBasedScriptLanguageTestProject : TemporaryStorageBackedTestProject
        {
            protected override IEnumerable<Type> GetScriptingTypes()
                => new[] { typeof(TestFileBasedScriptLanguage) };
        }
    }
}
