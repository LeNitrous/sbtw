// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Configuration;
using sbtw.Editor.Studios;

namespace sbtw.Editor.Tests.Studios
{
    public class TestStudioManager : StudioManager
    {
        public TestStudioManager(EditorConfigManager config)
            : base(config)
        {
        }

        public override IEnumerable<Studio> Supported => new[]
        {
            new Studio
            {
                Name = "test",
                FriendlyName = "Test"
            }
        };

        protected override bool IsSupported(Studio studio) => true;
    }
}
