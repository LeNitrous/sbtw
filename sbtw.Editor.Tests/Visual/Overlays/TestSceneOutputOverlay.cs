// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using NUnit.Framework;
using osu.Game.Tests.Visual;
using sbtw.Editor.Overlays;

namespace sbtw.Editor.Tests.Visual.Overlays
{
    [Ignore("Visual-only test")]
    public class TestSceneOutputOverlay : OsuTestScene
    {
        public TestSceneOutputOverlay()
        {
            OutputOverlay overlay;
            Add(overlay = new OutputOverlay());

            AddStep("toggle visibility", () => overlay.ToggleVisibility());
            AddStep("send message", () => overlay.Print("Hello World"));
            AddStep("send error message", () => overlay.Error(new Exception(), "Hello World"));
            AddStep("send debug message", () => overlay.Debug("Hello World"));
        }
    }
}
