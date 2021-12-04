// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Overlays;

namespace sbtw.Game.Screens.Edit.Setup
{
    public class SetupScreenHeader : OverlayHeader
    {
        protected override OverlayTitle CreateTitle() => new SetupScreenTitle();

        private class SetupScreenTitle : OverlayTitle
        {
            public SetupScreenTitle()
            {
                Title = "new project";
                IconTexture = "Icons/Hexacons/devtools";
            }
        }
    }
}
