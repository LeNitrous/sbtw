// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Edit;
using osuTK.Input;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class PlayButton : PlaybackControlItem
    {
        private readonly EditorClock clock;
        private readonly IconButton button;

        public PlayButton(EditorClock clock)
        {
            this.clock = clock;

            Width = 40;
            Child = button = new IconButton
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Action = togglePause,
                Enabled = { Value = clock.Track.Value is TrackBass },
            };
        }

        private void togglePause()
        {
            if (!button.Enabled.Value)
                return;

            if (clock.IsRunning)
                clock.Stop();
            else
                clock.Start();
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    togglePause();
                    return true;
            }

            return base.OnKeyDown(e);
        }

        protected override void Update()
        {
            base.Update();
            button.Icon = clock.IsRunning ? FontAwesome.Regular.PauseCircle : FontAwesome.Regular.PlayCircle;
        }
    }
}
