// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Scripts
{
    public interface IScriptedElement
    {
        string Path { get; }

        IScript Owner { get; }

        Layer Layer { get; }

        double StartTime { get; }
    }
}
