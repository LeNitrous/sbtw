// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Edit;
using osuTK.Input;

namespace sbtw.Editor.Graphics.UserInterface.Bottom
{
    public class PlayButton : BottomControlItem
    {
        private readonly IconButton button;

        [Resolved]
        private EditorClock clock { get; set; }

        public PlayButton()
        {
            Width = 40;
            Child = button = new IconButton
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Action = togglePause,
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            clock.Track.BindValueChanged(_ => Schedule(() => button.Enabled.Value = clock.Track.Value is TrackBass), true);
        }

        private void togglePause()
        {
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
                    button.TriggerClick();
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
