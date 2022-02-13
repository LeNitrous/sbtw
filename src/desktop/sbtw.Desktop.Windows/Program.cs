// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework;

namespace sbtw.Desktop.Windows
{
    public class Program
    {
        public static void Main()
        {
            using var host = Host.GetSuitableDesktopHost("sbtw");
            using var game = new WindowsEditor();
            host.Run(game);
        }
    }
}
