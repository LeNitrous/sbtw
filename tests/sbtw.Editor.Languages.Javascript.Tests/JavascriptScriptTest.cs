// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Linq;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Microsoft.EntityFrameworkCore.Internal;
using NUnit.Framework;
using sbtw.Editor.Languages.Javascript.Scripts;
using sbtw.Editor.Languages.Tests;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages.Javascript.Tests
{
    [TestFixture]
    public class JavascriptScriptTest : ScriptTest
    {
        protected override string Extension => "js";

        protected static V8ScriptEngine CreateEngine()
        {
            var engine = new V8ScriptEngine();
            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
            return engine;
        }

        protected override Script CreateScript(string path) => new JavascriptScript(CreateEngine(), string.Empty, path);
        protected override Stream GetStream(string path) => JavascriptTestResources.GetStream(path);

        [Test]
        public void TestScriptImporting()
        {
            Copy($"importable.{Extension}");
            var script = Generate("importing");
            Assert.That(script.Groups.Any(g => g.Name == "Hello World"), Is.True);
        }
    }
}
