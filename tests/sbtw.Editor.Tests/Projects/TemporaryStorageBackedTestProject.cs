// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace sbtw.Editor.Tests.Projects
{
    public class TemporaryStorageBackedTestProject : TestProject
    {
        protected override Storage CreateStorage()
            => new TemporaryNativeStorage($"test-project-{Guid.NewGuid()}");

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            (Files as TemporaryNativeStorage)?.Dispose();
        }
    }
}
