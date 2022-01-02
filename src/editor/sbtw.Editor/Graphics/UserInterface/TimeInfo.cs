// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Screens.Edit;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class TimeInfo : PlaybackControlItem, IHasContextMenu, IHasPopover
    {
        [Resolved]
        private GameHost host { get; set; }

        private readonly EditorClock clock;
        private readonly OsuSpriteText display;

        public TimeInfo(EditorClock clock)
        {
            this.clock = clock;

            Width = 100;
            Child = display = new OsuSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = OsuFont.GetFont(size: 18, fixedWidth: true),
            };
        }

        protected override void Update()
        {
            base.Update();
            display.Text = clock.CurrentTime.ToEditorFormattedString();
        }

        protected override bool OnClick(ClickEvent e)
        {
            this.ShowPopover();
            return true;
        }

        public Popover GetPopover() => new SeekToPopover(clock);

        public MenuItem[] ContextMenuItems => new[]
        {
            new OsuMenuItem(@"Copy as...")
            {
                Items = new[]
                {
                    new OsuMenuItem(@"Formatted time", MenuItemType.Standard, () => host.GetClipboard().SetText(clock.CurrentTime.ToEditorFormattedString())),
                    new OsuMenuItem(@"Milliseconds", MenuItemType.Standard, () => host.GetClipboard().SetText(Math.Round(clock.CurrentTime).ToString())),
                }
            },
            new OsuMenuItem(@"Seek to...", MenuItemType.Standard, () => this.ShowPopover()),
        };

        private class SeekToPopover : OsuPopover
        {
            private readonly EditorClock clock;

            public readonly OsuNumberBox NumberBox;

            public SeekToPopover(EditorClock clock)
            {
                this.clock = clock;

                Children = new Drawable[]
                {
                    new OsuSpriteText
                    {
                        Font = OsuFont.GetFont(size: 18),
                        Text = "Seek to (in milliseconds):",
                    },
                    NumberBox = new OsuNumberBox
                    {
                        CommitOnFocusLost = false,
                        Margin = new MarginPadding { Top = 25 },
                        Width = 200,
                    },
                };

                NumberBox.OnCommit += onNumberBoxCommit;
            }

            private void onNumberBoxCommit(TextBox sender, bool newText)
            {
                if (double.TryParse(sender.Text, out double time))
                    clock.Seek(Math.Clamp(time, 0, clock.Track.Value?.Length ?? 0));

                Hide();
            }
        }
    }
}
