// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using NUnit.Framework;
using osu.Game.Tests.Visual;
using sbtw.Editor.Overlays;

namespace sbtw.Editor.Tests.Visual.Overlays
{
    [Ignore("Visual-only test")]
    public class TestSceneEditorSettingsOverlay : OsuTestScene
    {
        public TestSceneEditorSettingsOverlay()
        {
            EditorSettingsOverlay settingsOverlay = new EditorSettingsOverlay();
            Add(settingsOverlay);

            AddStep("toggle visibility", () => settingsOverlay.ToggleVisibility());
        }
    }
}
