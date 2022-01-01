// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Scripts
{
    public interface IScriptedElementWithDuration : IScriptedElement
    {
        double EndTime { get; }

        double Duration => EndTime - StartTime;
    }
}
