// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using sbtw.Editor.Configuration;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public class GroupsToolbox : EditorTabbedToolbox<string>
    {
        private Bindable<Group> selected;

        public GroupsToolbox()
            : base("Groups")
        {
            AddTab("Entries", new GroupsTab());
        }


        [BackgroundDependencyLoader]
        private void load(EditorSessionStatics statics)
        {
            selected = statics.GetBindable<Group>(EditorSessionStatic.GroupSelected);
            selected.ValueChanged += e =>
            {
                if (e.NewValue != null)
                    AddTab("Elements", new GroupElementsTab());

                Current.Value = e.NewValue != null ? "Elements" : "Entries";
            };

            Current.Value = "Entries";
            Current.ValueChanged += e =>
            {
                if (e.NewValue != "Entries")
                    return;

                RemoveTab("Elements");
                selected.Value = null;
            };
        }

        protected override TabControl<string> CreateTabControl()
            => new GroupsBreadcrumbControl { Height = 24, RelativeSizeAxes = Axes.X };

        private class GroupsBreadcrumbControl : BreadcrumbControl<string>
        {
            protected override TabItem<string> CreateTabItem(string value)
                => new GroupsBreadcrumbTabItem(value);

            protected class GroupsBreadcrumbTabItem : BreadcrumbTabItem
            {
                private IBindable<Group> selected;

                public GroupsBreadcrumbTabItem(string value)
                    : base(value)
                {
                    Text.Font = Text.Font.With(size: 14);
                    Chevron.Y -= 2;
                }

                [BackgroundDependencyLoader]
                private void load(EditorSessionStatics statics)
                {
                    selected = statics.GetBindable<Group>(EditorSessionStatic.GroupSelected);

                    if (Value == "Elements")
                        Text.Text = selected.Value.Name;
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
