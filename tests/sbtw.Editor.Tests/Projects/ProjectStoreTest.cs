// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using NUnit.Framework;
using osu.Framework.Testing;
using osu.Game.Tests;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Tests.Projects
{
    public class ProjectStoreTest : GameHostTest
    {
        [Test]
        public void TestProjectCreation()
        {
            using var host = new CleanRunHeadlessGameHost(callingMethodName: nameof(ProjectStoreTest));
            using var storage = new TemporaryNativeStorage($"project-{Guid.NewGuid()}", host);

            try
            {
                var editor = LoadEditor(host);
                var manager = new ProjectStore(host, editor.Audio, editor.RulesetStore);
                var project = manager.Create("project", storage.GetFullPath("."));
                Assert.That(project, Is.Not.Null);
            }
            finally
            {
                host.Exit();
            }
        }

        [Test]
        public void TestProjectLoading()
        {
            using var host = new CleanRunHeadlessGameHost(callingMethodName: nameof(ProjectStoreTest));
            using var storage = new TemporaryNativeStorage($"project-{Guid.NewGuid()}", host);

            try
            {
                var editor = LoadEditor(host);
                var manager = new ProjectStore(host, editor.Audio, editor.RulesetStore);
                var project = manager.Create("project", storage.GetFullPath("."));

                Assert.That(project, Is.Not.Null);

                var loaded = manager.Load(storage.GetFullPath("./project.sbtw.json"));
                Assert.That(project, Is.Not.Null);
            }
            finally
            {
                host.Exit();
            }
        }
    }
}
