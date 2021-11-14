// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public interface IScriptedElementHasEndTime : IScriptedElementHasStartTime
    {
        double EndTime { get; }

        double Duration { get; }
    }
}
