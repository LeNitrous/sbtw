// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Tests.Visual;
using sbtw.Editor.Graphics.UserInterface;

namespace sbtw.Editor.Tests.Visual.UserInterface
{
    [Ignore("Visual-only test")]
    public class TestSceneMainMenuBar : OsuTestScene
    {
        public TestSceneMainMenuBar()
        {
            Add(new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 40,
                Child = new MainMenuBar(),
            });
        }
    }
}
