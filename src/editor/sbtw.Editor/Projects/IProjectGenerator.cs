// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;

namespace sbtw.Editor.Projects
{
    public interface IProjectGenerator
    {
        void Generate(Storage storage);
    }
}
