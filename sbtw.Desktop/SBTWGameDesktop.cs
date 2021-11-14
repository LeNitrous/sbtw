// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Reflection;
using osu.Framework.Platform;
using sbtw.Desktop.IO;
using sbtw.Game;

namespace sbtw.Desktop
{
    public class SBTWGameDesktop : SBTWGame
    {
        protected override string OpenFileDialog(IEnumerable<string> filters, string filterDescription)
            => TinyFileDialog.OpenFileDialog(filters, filterDescription);

        protected override string SaveFileDialog(string filename, IEnumerable<string> filters, string filterDescription)
            => TinyFileDialog.SaveFileDialog(filename, filters, filterDescription);

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            if (host.Window is not SDL2DesktopWindow window)
                return;

            window.CursorState |= CursorState.Hidden;
            window.Title = "sbtw!";

            window.SetIconFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), "icon.ico"));
        }
    }
}
