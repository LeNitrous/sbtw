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
    public class StudioManager : IStudioManager
    {
        public IEnumerable<Studio> Studios { get; }

        public Bindable<Studio> Current { get; } = new Bindable<Studio>();

        private readonly Bindable<string> current;
        private readonly Studio[] supported = new[]
        {
            new Studio
            {
                Name = "code",
                FriendlyName = "Visual Studio Code",
            },
            new Studio
            {
                Name = "code_insiders",
                FriendlyName = "Visual Studio Code Insiders"
            }
        };

        public StudioManager(EditorConfigManager manager)
        {
            var found = new List<Studio>();

            foreach (var studio in supported)
            {
                if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
                {
                    foreach (string path in Environment.GetEnvironmentVariable("PATH").Split(';'))
                    {
                        if (path.Contains(studio.FriendlyName) && !Studios.Any(s => s.FriendlyName == studio.FriendlyName))
                            found.Add(studio);
                    }
                }

                if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
                {
                    if (File.Exists($@"/usr/bin/{studio.Name}"))
                        found.Add(studio);
                }
            }

            Studios = found;

            current = manager.GetBindable<string>(EditorSetting.PreferredStudio);
            var preferred = Studios.FirstOrDefault(s => s.FriendlyName == current.Value);

            if (preferred == null)
                Current.Value = Studios.FirstOrDefault();

            Current.BindValueChanged(s => current.Value = s.NewValue?.FriendlyName ?? string.Empty, true);
        }
    }
}
