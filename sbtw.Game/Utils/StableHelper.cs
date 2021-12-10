// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace sbtw.Game.Utils
{
    public static class StableHelper
    {
        /// <summary>
        /// Returns the path to a stable installation or null if no stable installation was found.
        /// </summary>
        public static readonly string STABLE_PATH = get_stable_path();

        /// <summary>
        /// Returns whether there is an existing stable installation found.
        /// </summary>
        public static bool HAS_STABLE => !string.IsNullOrEmpty(STABLE_PATH);

        // https://github.com/ppy/osu/blob/master/osu.Desktop/OsuGameDesktop.cs
        private static string get_stable_path()
        {
            static bool checkExists(string p) => Directory.Exists(Path.Combine(p, "Songs")) || File.Exists(Path.Combine(p, "osu!.cfg"));

            string stableInstallPath;

            if (OperatingSystem.IsWindows())
            {
                try
                {
                    stableInstallPath = get_stable_install_path_from_registry();

                    if (!string.IsNullOrEmpty(stableInstallPath) && checkExists(stableInstallPath))
                        return stableInstallPath;
                }
                catch { }
            }

            stableInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"osu!");
            if (checkExists(stableInstallPath))
                return stableInstallPath;

            stableInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".osu");
            if (checkExists(stableInstallPath))
                return stableInstallPath;

            return null;
        }

        [SupportedOSPlatform("windows")]
        private static string get_stable_install_path_from_registry()
        {
            using RegistryKey key = Registry.ClassesRoot.OpenSubKey("osu");
            return key?.OpenSubKey(@"shell\open\command")?.GetValue(string.Empty)?.ToString()?.Split('"')[1].Replace("osu!.exe", "");
        }
    }
}
