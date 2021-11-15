// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public interface IScriptedElementHasStartTime : IScriptedElement
    {
        double StartTime { get; }
    }
}
