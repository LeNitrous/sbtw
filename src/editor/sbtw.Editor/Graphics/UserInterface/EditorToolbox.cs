// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Input.Events;
using osu.Game.Screens.Play.PlayerSettings;

namespace sbtw.Editor.Graphics.UserInterface
{
    public abstract class EditorToolbox : PlayerSettingsGroup
    {
        protected EditorToolbox(string title)
            : base(title)
        {
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
