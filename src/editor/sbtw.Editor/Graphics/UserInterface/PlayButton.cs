// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
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
        private readonly IconButton button;

        [Resolved]
        private Bindable<EditorClock> clock { get; set; }

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
            clock.BindValueChanged(e => Schedule(() => button.Enabled.Value = clock.Value.Track.Value is TrackBass), true);
        }

        private void togglePause()
        {
            if (!button.Enabled.Value)
                return;

            if (clock.Value.IsRunning)
                clock.Value.Stop();
            else
                clock.Value.Start();
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
            button.Icon = clock.Value.IsRunning ? FontAwesome.Regular.PauseCircle : FontAwesome.Regular.PlayCircle;
        }
    }
}
