// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Scripts.Elements
{
    public interface IScriptElement
    {
        string Path { get; }
        Group Group { get; }
        Layer Layer { get; }
        double StartTime { get; }
    }
}
