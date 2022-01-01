// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using osu.Framework;
using sbtw.Editor.Configuration;
using sbtw.Editor.Studios;

namespace sbtw.Desktop.Studios
{
    public class DesktopStudioManager : StudioManager
    {
        public DesktopStudioManager(EditorConfigManager config)
            : base(config)
        {
        }

        public override IEnumerable<Studio> Supported => new[]
        {
            new Studio
            {
                Name = "code",
                FriendlyName = "Microsoft VS Code",
            },
            new Studio
            {
                Name = "atom",
                FriendlyName = "Atom",
            },
            new Studio
            {
                Name = "subl",
                FriendlyName = "Sublime Text",
            },
        };

        protected override bool IsSupported(Studio studio)
        {
            if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            {
                foreach (string path in Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User).Split(';'))
                {
                    if (path.Contains(studio.FriendlyName))
                        return true;
                }

                return false;
            }

            return RuntimeInfo.OS == RuntimeInfo.Platform.Linux &&
                (File.Exists($@"/usr/bin/{studio.Name}") || File.Exists($@"/usr/local/bin/{studio.Name}"));
        }
    }
}
