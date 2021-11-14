// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Edit.Setup;

namespace sbtw.Game.Screens.Setup
{
    public class ConfirmSection : SetupSection
    {
        public override LocalisableString Title => string.Empty;

        public Action ConfirmAction;

        [BackgroundDependencyLoader]
        private void load(OsuColour colour)
        {
            Children = new Drawable[]
            {
                new OsuButton
                {
                    Text = "Create",
                    Width = 200,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    BackgroundColour = colour.Yellow,
                    Action = ConfirmAction,
                },
            };
        }
    }
}
