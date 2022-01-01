// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK.Graphics;

namespace sbtw.Editor.Graphics.UserInterface
{
    public abstract class EditorTabbedToolbox<T> : EditorToolbox
    {
        protected override Container<Drawable> Content { get; }

        private readonly TabControl<T> tabControl;
        private readonly Dictionary<T, Drawable> tabs = new Dictionary<T, Drawable>();

        public EditorTabbedToolbox(string title)
            : base(title)
        {
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
                        tabControl = CreateTabControl(),
                    }
                },
                Content = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 500,
                },
            };

            tabControl.Current.ValueChanged += e => SetTab(e.NewValue);
        }

        protected abstract TabControl<T> CreateTabControl();

        public void AddTab(T identifier, Drawable tab)
        {
            tabs.Add(identifier, tab);
            tabControl.AddItem(identifier);

            if (!Contains(tab))
                Add(tab);
        }

        public bool RemoveTab(T identifier)
        {
            if (!tabs.TryGetValue(identifier, out var drawable))
                return false;

            Remove(drawable);
            tabs.Remove(identifier);
            tabControl.RemoveItem(identifier);

            return true;
        }

        public void SetTab(T identifier)
        {
            if (!tabs.TryGetValue(identifier, out var drawable))
                return;

            tabControl.Current.Value = identifier;

            foreach (var tab in tabs.Values)
                tab.Hide();

            drawable.Show();
        }
    }
}
