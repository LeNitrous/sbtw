// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Overlays;
using osuTK;

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public abstract class EditorToolbox : SettingsToolboxGroup
    {
        protected EditorToolbox(string title)
            : base(title)
        {
            (base.Content as FillFlowContainer).Spacing = new Vector2(0, 5);
        }

        protected override void LoadComplete()
        {
        }

        protected override bool OnHover(HoverEvent e)
        {
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
        }
    }
}
