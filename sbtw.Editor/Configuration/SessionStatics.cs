// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;
using osu.Game.Configuration;

namespace sbtw.Editor.Configuration
{
    public class SessionStatics : InMemoryConfigManager<SessionStatic>
    {
        protected override void InitialiseDefaults() => Reset();

        public void Reset()
        {
            ensure_default(SetDefault(SessionStatic.ShowInterface, true));
            ensure_default(SetDefault(SessionStatic.ShowPlayfield, true));
        }

        private static void ensure_default<T>(Bindable<T> bindable) => bindable.SetDefault();
    }

    public enum SessionStatic
    {
        ShowInterface,
        ShowPlayfield,
    }
}
