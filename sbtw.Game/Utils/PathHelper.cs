// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using osu.Framework;

namespace sbtw.Game.Utils
{
    public static class PathHelper
    {
        public static IEnumerable<string> GetEnvironmentPaths()
        {
            char separator = RuntimeInfo.OS == RuntimeInfo.Platform.Windows ? ';' : ':';
            foreach (string path in Environment.GetEnvironmentVariable("PATH").Split(separator))
                yield return path;
        }
    }
}
