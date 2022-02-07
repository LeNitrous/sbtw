// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework;

namespace sbtw.Editor.Tests
{
    public static class VisualTestRunner
    {
        [STAThread]
        public static int Main()
        {
            using var host = Host.GetSuitableDesktopHost(@"sbtw");
            host.Run(new EditorTestBrowser());
            return 0;
        }
    }
}
