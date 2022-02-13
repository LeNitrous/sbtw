// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace sbtw.Editor.Tests.Projects
{
    public class TemporaryStorageBackedTestProject : TestProject
    {
        private TemporaryNativeStorage storage;

        protected override Storage CreateStorage()
            => storage = new TemporaryNativeStorage($"test-project-{Guid.NewGuid()}");

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            storage?.Dispose();
        }
    }
}
