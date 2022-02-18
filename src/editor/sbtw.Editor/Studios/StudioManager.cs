// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework;
using osu.Framework.Bindables;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Studios
{
    public class StudioManager
    {
        public IReadOnlyList<Studio> Studios => studios;
        public readonly Bindable<Studio> Current = new NonNullableBindable<Studio>(new NoStudio());

        private readonly Bindable<string> currentString;
        private readonly List<Studio> studios = new List<Studio>();
        private readonly IReadOnlyList<Studio> supported = new Studio[]
        {
            new VSCodeInsidersStudio(),
            new VSCodeStudio(),
            new SublimeStudio(),
            new NppStudio(),
        };

        public StudioManager(EditorConfigManager config)
        {
            foreach (var studio in supported)
            {
                foreach (var path in Environment.GetEnvironmentVariable("PATH").Split(separator))
                {
                    if (File.Exists(Path.Combine(path, studio.Name)))
                        studios.Add(studio);
                }
            }

            currentString = config.GetBindable<string>(EditorSetting.PreferredStudio);
            var preferred = studios.FirstOrDefault(s => s.FriendlyName == currentString.Value);

            if (preferred != null)
                Current.Value = preferred;

            Current.BindValueChanged(s => currentString.Value = s.NewValue?.FriendlyName ?? string.Empty);
        }

        private static readonly char separator = RuntimeInfo.OS == RuntimeInfo.Platform.Windows ? ';' : ':';
    }
}
