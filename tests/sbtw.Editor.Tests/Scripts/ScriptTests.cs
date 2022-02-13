// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using NUnit.Framework;
using sbtw.Editor.Assets;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Tests.Projects;

namespace sbtw.Editor.Tests.Scripts
{
    public class ScriptTests
    {
        [Test]
        public void TestScriptException()
        {
            var result = run(null, null, s => s.GetGroup("test"));

            Assert.That(result.Faulted, Is.True);
            Assert.That(result.Exception, Is.InstanceOf<ScriptExecutionException>());
        }

        [Test]
        public void TestScriptGroupProvider()
        {
            var provider = new TestProject();
            var globals = new ScriptGlobals { GroupProvider = provider };
            var script = run(provider, globals, s => s.GetGroup("test"));

            Assert.That(script.Faulted, Is.False);
            Assert.That(provider.Groups, Is.Not.Empty);
        }

        [Test]
        public void TestScriptLogger()
        {
            var provider = new TestProject();
            var globals = new ScriptGlobals { Logger = provider };
            var script = run(provider, globals, s => s.Log("Hello World"));

            Assert.That(script.Faulted, Is.False);
            Assert.That(provider.Logs, Is.Not.Empty);
            Assert.That(provider.Logs.First(), Is.EqualTo("Hello World"));
        }

        [Test]
        public void TestScriptFileProvider()
        {
            var provider = new TestProject();
            var globals = new ScriptGlobals { FileProvider = provider };
            var result = run(provider, globals, s => s.Fetch("test.png"));

            Assert.That(result.Faulted, Is.False);
        }

        [Test]
        public void TestScriptAssetProvider()
        {
            var provider = new TestProject();
            var globals = new ScriptGlobals { AssetProvider = provider };
            var result = run(provider, globals, s => s.GetAsset("test.png", new Rectangle()));

            Assert.That(result.Faulted, Is.False);
            Assert.That(provider.Assets, Is.Not.Empty);
            Assert.That(provider.Assets.First(), Is.InstanceOf<Rectangle>());
            Assert.That(provider.Assets.First().Path, Is.EqualTo("test.png"));
        }

        private static ScriptExecutionResult run(IProject project, ScriptGlobals globals, Action<dynamic> action) => new TestScriptLanguage(project)
        {
            Scripts = new[]
            {
                new TestScript { Action = action }
            }
        }.Execute(globals).FirstOrDefault();
    }
}
