// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using NUnit.Framework;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Game.Tests;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Tests.Projects
{
    public class ProjectManagerTest
    {
        [Test]
        public void TestProjectCreation()
        {
            using var host = new CleanRunHeadlessGameHost(nameof(ProjectManagerTest));
            using var storage = new TemporaryNativeStorage($"project-{Guid.NewGuid()}", host);

            try
            {
                var manager = new ProjectManager(host);
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
            using var host = new CleanRunHeadlessGameHost(nameof(ProjectManagerTest));
            using var storage = new TemporaryNativeStorage($"project-{Guid.NewGuid()}", host);

            try
            {
                var manager = new ProjectManager(host);
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

        private void createProjectAndPerform(Action<ProjectManager, Storage> perform)
        {
            using var host = new CleanRunHeadlessGameHost(nameof(ProjectManagerTest));
            using var storage = new TemporaryNativeStorage($"project-{Guid.NewGuid()}", host);

            try
            {
                var manager = new ProjectManager(host);
                var project = manager.Create("project", storage.GetFullPath("."));

                Assert.That(project, Is.Not.Null);

                perform.Invoke(manager, storage);
            }
            finally
            {
                host.Exit();
            }
        }
    }
}
