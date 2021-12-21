// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using NUnit.Framework;
using sbtw.Editor.Languages.Javascript.Scripts;

namespace sbtw.Editor.Languages.Javascript.Tests
{
    public class TestScripts
    {
        private static readonly Typescript typescript = new Typescript(new V8ScriptEngine());

        [Test]
        public void TestJavascriptScript()
        {
            var script = new JavascriptScript(createEngine(), "script", SetUpTests.Storage.GetFullPath("script.js"));
            var result = script.Generate();

            Assert.That(result.Groups.Any(), Is.True);
        }

        [Test]
        public void TestTypescriptScript()
        {
            var script = new TypescriptScript(createEngine(), typescript, "script", SetUpTests.Storage.GetFullPath("script.ts"));
            var result = script.Generate();

            Assert.That(result.Groups.Any(), Is.True);
        }

        [Test]
        public void TestJavascriptScriptShouldThrow()
        {
            var script = new JavascriptScript(createEngine(), "script", SetUpTests.Storage.GetFullPath("throw.js"));
            Assert.Throws<ScriptEngineException>(() => { script.Generate(); });
        }

        [Test]
        public void TestTypescriptScriptShouldThrow()
        {
            var script = new TypescriptScript(createEngine(), typescript, "script", SetUpTests.Storage.GetFullPath("throw.ts"));
            Assert.Throws<ScriptEngineException>(() => { script.Generate(); });
        }

        [Test]
        public void TestJavascriptScriptImporting()
        {
            var script = new JavascriptScript(createEngine(), "script", SetUpTests.Storage.GetFullPath("importing.js"));
            var result = script.Generate();
            Assert.That(result.Groups.Any(g => g.Name == "Hello World"), Is.True);
        }

        [Test]
        public void TestTypescriptScriptImporting()
        {
            var script = new TypescriptScript(createEngine(), typescript, "script", SetUpTests.Storage.GetFullPath("importing.ts"));
            var result = script.Generate();
            Assert.That(result.Groups.Any(g => g.Name == "Hello World"), Is.True);
        }

        private static V8ScriptEngine createEngine()
        {
            var engine = new V8ScriptEngine();
            engine.DocumentSettings.AccessFlags |= DocumentAccessFlags.EnableFileLoading;
            return engine;
        }
    }
}
