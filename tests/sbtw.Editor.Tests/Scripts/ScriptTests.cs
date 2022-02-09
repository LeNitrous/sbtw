// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Linq;
using NUnit.Framework;
using sbtw.Editor.Assets;
using sbtw.Editor.Tests.Projects;

namespace sbtw.Editor.Tests.Scripts
{
    public class ScriptTests
    {
        [Test]
        public void TestScriptException()
        {
            var script = new TestScript { Action = s => s.GetGroup("test") };
            script.Execute();

            Assert.That(script.Faulted, Is.True);
            Assert.That(script.Exception, Is.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void TestScriptGroupProvider()
        {
            var provider = new TestProject();
            var script = new TestScript { Action = s => s.GetGroup("test") };
            script.GroupProvider = provider;
            script.Execute();

            Assert.That(script.Faulted, Is.False);
            Assert.That(provider.Groups, Is.Not.Empty);
        }

        [Test]
        public void TestScriptLogger()
        {
            var provider = new TestProject();
            var script = new TestScript { Action = s => s.Log("Hello World") };
            script.Logger = provider;
            script.Execute();

            Assert.That(script.Faulted, Is.False);
            Assert.That(provider.Logs, Is.Not.Empty);
            Assert.That(provider.Logs.First(), Is.EqualTo("Hello World"));
        }

        [Test]
        public void TestScriptFileProvider()
        {
            var provider = new TestProject();
            var script = new TestScript { Action = s => s.Fetch("test.png") };
            script.FileProvider = provider;
            script.Execute();

            Assert.That(script.Faulted, Is.False);
        }

        [Test]
        public void TestScriptAssetProvider()
        {
            var provider = new TestProject();
            var script = new TestScript { Action = s => s.GetAsset("test.png", new Rectangle()) };
            script.AssetProvider = provider;
            script.Execute();

            Assert.That(script.Faulted, Is.False);
            Assert.That(provider.Assets, Is.Not.Empty);
            Assert.That(provider.Assets.First(), Is.InstanceOf<Rectangle>());
            Assert.That(provider.Assets.First().Path, Is.EqualTo("test.png"));
        }
    }
}
