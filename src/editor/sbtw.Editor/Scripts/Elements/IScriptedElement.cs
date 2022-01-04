// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Scripts.Elements
{
    public interface IScriptedElement
    {
        string Path { get; }

        Script Owner { get; }

        string Group { get; }

        Layer Layer { get; }

        double StartTime { get; }
    }
}
