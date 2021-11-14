// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Edit;

namespace sbtw.Game.Screens.Edit
{
    public class GroupsToolboxGroup : ToolboxGroup
    {
        public IBindableList<string> Items => list.Items;

        private readonly GroupList list;

        public GroupsToolboxGroup()
            : base("Groups")
        {
            Child = list = new GroupList
            {
                RelativeSizeAxes = Axes.X,
                Height = 250,
            };
        }

        private class GroupList : OsuRearrangeableListContainer<string>
        {
            protected override OsuRearrangeableListItem<string> CreateOsuDrawable(string item)
                => new GroupListItem(item);
        }

        private class GroupListItem : OsuRearrangeableListItem<string>
        {
            public GroupListItem(string item)
                : base(item)
            {
            }

            protected override Drawable CreateContent() => new OsuSpriteText
            {
                Text = Model,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 18 },
            };
        }
    }
}
