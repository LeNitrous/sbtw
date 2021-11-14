// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework;

namespace sbtw.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using var host = Host.GetSuitableHost("sbtw");
            host.Run(new SBTWGameDesktop());
        }
    }
}
