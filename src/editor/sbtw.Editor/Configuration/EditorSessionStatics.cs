// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;
using osu.Game.Configuration;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Configuration
{
    public class EditorSessionStatics : InMemoryConfigManager<EditorSessionStatic>
    {
        protected override void InitialiseDefaults() => Reset();

        public void Reset()
        {
            ensure_default(SetDefault(EditorSessionStatic.ShowInterface, true));
            ensure_default(SetDefault(EditorSessionStatic.ShowPlayfield, true));
            ensure_default(SetDefault(EditorSessionStatic.StoryboardShowVideo, true));
            ensure_default(SetDefault(EditorSessionStatic.StoryboardShowBackground, true));
            ensure_default(SetDefault(EditorSessionStatic.StoryboardShowFailing, true));
            ensure_default(SetDefault(EditorSessionStatic.StoryboardShowPassing, true));
            ensure_default(SetDefault(EditorSessionStatic.StoryboardShowForeground, true));
            ensure_default(SetDefault(EditorSessionStatic.StoryboardShowOverlay, true));
            ensure_default(SetDefault(EditorSessionStatic.TrackRate, 1.0));
            ensure_default(SetDefault(EditorSessionStatic.TrackRateAffectsPitch, true));
            ensure_default(SetDefault<Group>(EditorSessionStatic.GroupSelected, null));
        }

        private static void ensure_default<T>(Bindable<T> bindable) => bindable.SetDefault();
    }

    public enum EditorSessionStatic
    {
        ShowInterface,
        ShowPlayfield,
        StoryboardShowVideo,
        StoryboardShowBackground,
        StoryboardShowFailing,
        StoryboardShowPassing,
        StoryboardShowForeground,
        StoryboardShowOverlay,
        TrackRate,
        TrackRateAffectsPitch,
        GroupSelected,
    }
}
