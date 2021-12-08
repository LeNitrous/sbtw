// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Edit;

namespace sbtw.Game.Screens.Edit.Menus
{
    public class RateControl : BottomMenuBarItem, IHasContextMenu
    {
        private readonly OsuSpriteText text;
        private readonly Dictionary<double, TernaryStateRadioMenuItem> itemMap = new Dictionary<double, TernaryStateRadioMenuItem>();
        private readonly BindableNumber<double> frequency = new BindableNumber<double>(1);
        private readonly MenuItem[] menuItems;

        [Resolved]
        private EditorClock clock { get; set; }

        public RateControl()
        {
            Width = 60;
            Child = text = new OsuSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = OsuFont.GetFont(size: 18),
            };

            menuItems = new[]
            {
                createMenuItem(1.5),
                createMenuItem(1.0),
                createMenuItem(0.75),
                createMenuItem(0.5),
                createMenuItem(0.25),
            };

            frequency.BindValueChanged(e =>
            {
                text.Text = $"{e.NewValue * 100}%";
                foreach ((double rate, TernaryStateRadioMenuItem item) in itemMap)
                    item.State.Value = rate == e.NewValue ? TernaryState.True : TernaryState.False;
            }, true);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            clock.Track.Value.AddAdjustment(AdjustableProperty.Frequency, frequency);
        }

        public MenuItem[] ContextMenuItems => menuItems;

        protected override bool OnClick(ClickEvent e)
        {
            var keys = itemMap.Keys.ToList();

            int index = (keys.IndexOf(frequency.Value) + 1) % itemMap.Count;
            frequency.Value = keys[index];

            return base.OnClick(e);
        }

        protected override void Dispose(bool isDisposing)
        {
            clock.Track.Value.RemoveAdjustment(AdjustableProperty.Frequency, frequency);
            base.Dispose(isDisposing);
        }

        private TernaryStateRadioMenuItem createMenuItem(double rate)
        {
            var item = new TernaryStateRadioMenuItem($"{rate * 100}%", MenuItemType.Standard, _ => frequency.Value = rate);
            itemMap[rate] = item;
            return item;
        }
    }
}
