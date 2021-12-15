// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Bindables;
using sbtw.Editor.Studios;

namespace sbtw.Editor.Tests.Studios
{
    public class TestStudioManager : IStudioManager
    {
        public IEnumerable<Studio> Studios { get; }
        public Bindable<Studio> Current { get; }

        private readonly Studio studio = new Studio
        {
            Name = "test",
            FriendlyName = "Test"
        };

        public TestStudioManager()
        {
            Studios = new[] { studio };
            Current = new Bindable<Studio>(studio);
        }
    }
}
