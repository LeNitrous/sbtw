// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Edit;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class PlaybackControl : BottomMenuBarItem
    {
        [Resolved]
        private EditorClock clock { get; set; }

        private IconButton button;

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = 40;
            Child = button = new IconButton
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Action = togglePause,
            };
        }

        private void togglePause()
        {
            if (clock.IsRunning)
                clock.Stop();
            else
                clock.Start();
        }

        protected override void Update()
        {
            base.Update();
            button.Icon = clock.IsRunning ? FontAwesome.Regular.PauseCircle : FontAwesome.Regular.PlayCircle;
        }
    }
}
