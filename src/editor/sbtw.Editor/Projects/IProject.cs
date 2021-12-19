// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Projects
{
    public interface IProject
    {
        string Name { get; }

        string Path { get; }

        bool Save();
    }
}