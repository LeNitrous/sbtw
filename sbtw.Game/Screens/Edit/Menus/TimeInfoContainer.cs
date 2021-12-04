// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Edit;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class TimeInfoContainer : BottomMenuBarItem, IHasContextMenu
    {
        [Resolved]
        private EditorClock clock { get; set; }

        private readonly OsuSpriteText display;

        public TimeInfoContainer()
        {
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

        public MenuItem[] ContextMenuItems => new[]
        {
            new OsuMenuItem(@"Copy...")
            {
                Items = new[]
                {
                    new OsuMenuItem(@"As formatted time"),
                    new OsuMenuItem(@"As milliseconds"),
                }
            },
            new OsuMenuItem(@"Move to...")
        };
    }
}
