// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;

namespace sbtw.Editor.Graphics.UserInterface
{
    public partial class Toolbox : EditorTabbedToolbox<string>
    {
        public Toolbox()
            : base("Tools")
        {
            AddTab(@"Layers", new LayersToolboxTab());
            AddTab(@"Groups", new GroupsToolboxTab());
            AddTab(@"Scripts", new ScriptsToolboxTab());
        }

        protected override TabControl<string> CreateTabControl() => new ViewToolboxTabControl { RelativeSizeAxes = Axes.Both };

        private class ViewToolboxTabControl : OsuTabControl<string>
        {
            protected override TabItem<string> CreateTabItem(string value) => new ToolBoxTabItem(value);

            private class ToolBoxTabItem : OsuTabItem
            {
                public ToolBoxTabItem(string value)
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
