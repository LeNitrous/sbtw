// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;

namespace sbtw.Editor.Projects
{
    public class DummyProject : IProject
    {
        public string Name => @"no project";
        public string Path => string.Empty;
        public IEnumerable<string> Exclude { get; }
    }
}
