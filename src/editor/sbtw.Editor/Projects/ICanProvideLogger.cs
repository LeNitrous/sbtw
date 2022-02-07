// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Logging;

namespace sbtw.Editor.Projects
{
    public interface ICanProvideLogger
    {
        void Log(object message, LogLevel level);
    }
}
