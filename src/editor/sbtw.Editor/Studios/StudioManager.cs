// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Studios
{
    public abstract class StudioManager
    {
        public IReadOnlyList<Studio> Studios => studios;
        public abstract IEnumerable<Studio> Supported { get; }
        public readonly Bindable<Studio> Current = new Bindable<Studio>();

        private readonly Bindable<string> currentString;
        private readonly List<Studio> studios = new List<Studio>();

        public StudioManager(EditorConfigManager config)
        {
            foreach (var studio in Supported)
            {
                if (!Studios.Any(s => s.Equals(s)) && IsSupported(studio))
                    studios.Add(studio);
            }

            currentString = config.GetBindable<string>(EditorSetting.PreferredStudio);
            var preferred = studios.FirstOrDefault(s => s.FriendlyName == currentString.Value);

            if (preferred == null)
                Current.Value = studios.FirstOrDefault();

            Current.BindValueChanged(s => currentString.Value = s.NewValue?.FriendlyName ?? string.Empty, true);
        }

        protected abstract bool IsSupported(Studio studio);
    }
}
