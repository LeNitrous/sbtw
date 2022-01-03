// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

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
using osu.Game.Screens.Edit.Components.Menus;
using osuTK.Input;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class RateSelector : PlaybackControlItem, IHasContextMenu
    {
        private readonly OsuSpriteText text;
        private readonly Dictionary<double, TernaryStateRadioMenuItem> itemMap = new Dictionary<double, TernaryStateRadioMenuItem>();
        private readonly Bindable<double> tempo = new Bindable<double>(1);
        private readonly Bindable<double> frequency = new Bindable<double>(1);
        private Bindable<double> rate;
        private Bindable<bool> affectsTempo;
        private MenuItem[] menuItems;

        [Resolved]
        private Bindable<EditorClock> clock { get; set; }

        public RateSelector()
        {
            Width = 60;
            Child = text = new OsuSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = OsuFont.GetFont(size: 18),
            };
        }

        [BackgroundDependencyLoader]
        private void load(EditorSessionStatics statics)
        {
            affectsTempo = statics.GetBindable<bool>(EditorSessionStatic.TrackRateAffectsPitch);
            affectsTempo.ValueChanged += _ => updateRate();

            rate = statics.GetBindable<double>(EditorSessionStatic.TrackRate);
            rate.ValueChanged += _ => updateRate();

            menuItems = new MenuItem[]
            {
                new ToggleMenuItem("Change Pitch", MenuItemType.Standard) { State = { BindTarget = affectsTempo } },
                new EditorMenuItemSpacer(),
                createMenuItem(1.5),
                createMenuItem(1.0),
                createMenuItem(0.75),
                createMenuItem(0.5),
                createMenuItem(0.25),
            };

            clock.BindValueChanged(e =>
            {
                e.OldValue.Track.Value?.RemoveAdjustment(AdjustableProperty.Frequency, frequency);
                e.OldValue.Track.Value?.RemoveAdjustment(AdjustableProperty.Tempo, tempo);
                e.NewValue.Track.Value?.AddAdjustment(AdjustableProperty.Frequency, frequency);
                e.NewValue.Track.Value?.AddAdjustment(AdjustableProperty.Tempo, tempo);
            }, true);

            updateRate();
        }

        public MenuItem[] ContextMenuItems => menuItems;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    changeRate(1);
                    return true;

                case MouseButton.Middle:
                    changeRate(-1);
                    return true;
            }

            return base.OnMouseDown(e);
        }

        private void changeRate(int direction)
        {
            var keys = itemMap.Keys.ToList();
            int index = (((keys.IndexOf(rate.Value) + direction) % itemMap.Count) + itemMap.Count) % itemMap.Count;
            rate.Value = keys[index];
        }

        protected override void Dispose(bool isDisposing)
        {
            clock.Value.Track.Value?.RemoveAdjustment(AdjustableProperty.Frequency, frequency);
            clock.Value.Track.Value?.RemoveAdjustment(AdjustableProperty.Tempo, tempo);
            base.Dispose(isDisposing);
        }

        private TernaryStateRadioMenuItem createMenuItem(double newRate)
        {
            var item = new TernaryStateRadioMenuItem($"{newRate * 100}%", MenuItemType.Standard, _ => rate.Value = newRate);
            itemMap[newRate] = item;
            return item;
        }

        private void updateRate()
        {
            text.Text = $"{rate.Value * 100}%";
            foreach ((double rateSetting, TernaryStateRadioMenuItem item) in itemMap)
                item.State.Value = rateSetting == rate.Value ? TernaryState.True : TernaryState.False;

            if (!affectsTempo.Value)
            {
                frequency.Value = 1.0;
                tempo.Value = rate.Value / 1.0;
            }
            else
            {
                frequency.Value = rate.Value;
                tempo.Value = 1.0;
            }
        }
    }
}
