// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Overlays;

namespace sbtw.Editor.Overlays.Setup
{
    public class SetupHeader : OverlayHeader
    {
        protected override OverlayTitle CreateTitle() => new SetupHeaderTitle();

        private class SetupHeaderTitle : OverlayTitle
        {
            public SetupHeaderTitle()
            {
                Title = "new project";
                IconTexture = "Icons/Hexacons/devtools";
            }
        }
    }
}
