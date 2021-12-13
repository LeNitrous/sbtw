// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Projects
{
    public class DummyProject : IProject
    {
        public string Name => @"No project";
        public string Path => string.Empty;

        public bool Save() => true;
    }
}
