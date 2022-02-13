// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using osuTK.Graphics;

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public abstract class EditorTabbedToolbox<T> : EditorToolbox, IHasCurrentValue<T>
    {
        public Bindable<T> Current
        {
            get => control.Current;
            set => control.Current = value;
        }

        protected override Container<Drawable> Content { get; }
        private readonly TabControl<T> control;
        private readonly Dictionary<T, Drawable> tabs = new Dictionary<T, Drawable>();

        public EditorTabbedToolbox(string title)
            : base(title)
        {
            control = CreateTabControl();

            base.Content.Children = new Drawable[]
            {
                new Container
                {
                    Height = 24,
                    RelativeSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Height = 1,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Colour = Color4.White.Opacity(0.2f),
                            RelativeSizeAxes = Axes.X,
                        },
                        control
                    }
                },
                Content = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                },
            };

            Current.ValueChanged += e => setTab(e.NewValue);
        }

        protected virtual TabControl<T> CreateTabControl()
            => new EditorTabbedToolboxTabControl { RelativeSizeAxes = Axes.Both };

        protected void AddTab(T tab, EditorTabbedToolboxTab drawable)
        {
            tabs.Add(tab, drawable);
            control.AddItem(tab);

            if (!Contains(drawable))
                Add(drawable);
        }

        protected bool RemoveTab(T tab)
        {
            if (!tabs.TryGetValue(tab, out var drawable))
                return false;

            drawable.Expire();
            tabs.Remove(tab);
            control.RemoveItem(tab);

            return true;
        }

        private void setTab(T tab)
        {
            if (!tabs.TryGetValue(tab, out var drawable))
                return;

            control.Current.Value = tab;

            foreach (var other in tabs.Values)
                other.Hide();

            drawable.Show();
        }

        protected class EditorTabbedToolboxTabControl : OsuTabControl<T>
        {
            protected override TabItem<T> CreateTabItem(T value)
                => new EditorTabbedToolboxTabItem(value);

            protected class EditorTabbedToolboxTabItem : OsuTabItem
            {
                public EditorTabbedToolboxTabItem(T value)
                    : base(value)
                {
                }

                protected override bool OnHover(HoverEvent e)
                {
                    if (Active.Value)
                        FadeHovered();

                    return false;
                }
            }
        }
    }
}
