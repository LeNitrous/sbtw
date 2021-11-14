// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace sbtw.Game.Projects
{
    public static class ProjectHelper
    {
        /// <summary>
        /// Returns an enumerable list of paths to installed code editors based on the "PATH" environment variable.
        /// </summary>
        public static readonly Dictionary<string, string> EDITORS = get_installed_code_editor_paths();

        /// <summary>
        /// Returns whether there is an existing stable installation found.
        /// </summary>
        public static bool HAS_STABLE => !string.IsNullOrEmpty(STABLE_PATH);

        /// <summary>
        /// Returns the path to a stable installation or null if no stable installation was found.
        /// </summary>
        public static readonly string STABLE_PATH = get_stable_path();

        /// <summary>
        /// Returns whether the .NET driver is installed or not.
        /// </summary>
        public static bool HAS_DOTNET => !string.IsNullOrEmpty(DOTNET_PATH);

        /// <summary>
        /// Returns the path to the .NET driver.
        /// </summary>
        public static readonly string DOTNET_PATH = Path.Combine(get_dotnet_path(), "dotnet");

        /// <summary>
        /// Invoked when the .NET driver logs a standard message.
        /// </summary>
        public static event Action<string> OnDotNetMessage;

        /// <summary>
        /// Invoked when the .NET driver logs an error message.
        /// </summary>
        public static event Action<string> OnDotNetError;

        /// <summary>
        /// Invoked when the .NET driver starts.
        /// </summary>
        public static event Action<string> OnDotNetStart;

        /// <summary>
        /// Invoked when the .NET driver exits.
        /// </summary>
        public static event Action OnDotNetExit;

        /// <summary>
        /// Executes the .NET driver to build the project.
        /// </summary>
        /// <param name="path">The path to the project.</param>
        public static void Build(string path) => start_dotnet($"build {path}");

        /// <summary>
        /// Executes the .NET driver to clean the project output.
        /// </summary>
        /// <param name="path">The path to the project.</param>
        public static void Clean(string path) => start_dotnet($"clean {path}");

        /// <summary>
        /// Executes the .NET driver to restore project dependencies.
        /// </summary>
        /// <param name="path">The path to the project.</param>>
        public static void Restore(string path) => start_dotnet($"restore {path}");

        private static Process current;

        private static void start_dotnet(string args)
        {
            if (current != null || !HAS_DOTNET)
                return;

            current = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = DOTNET_PATH,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    ErrorDialog = false,
                }
            };

            current.Exited += (_, __) =>
            {
                current.Dispose();
                current = null;
                OnDotNetExit?.Invoke();
            };

            current.OutputDataReceived += (_, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    OnDotNetMessage?.Invoke(args.Data);
            };

            current.ErrorDataReceived += (_, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    OnDotNetError?.Invoke(args.Data);
            };

            current.Start();
            current.BeginOutputReadLine();
            current.BeginErrorReadLine();

            OnDotNetStart?.Invoke(args);
        }

        private static Dictionary<string, string> get_installed_code_editor_paths()
        {
            var found = new Dictionary<string, string>();

            string[] known = new[]
            {
                "Microsoft VS Code",
                "Microsoft VS Code Insiders",
            };

            string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');

            foreach (string path in paths)
            {
                foreach (string editor in known)
                {
                    if (path.Contains($@"\{editor}\") && !found.ContainsKey(path))
                        found.Add(Path.Combine(path, "code"), editor);
                }
            }

            return found;
        }

        private static string get_dotnet_path()
        {
            string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');

            foreach (string path in paths)
            {
                if (path.Contains(@"\dotnet"))
                    return path;
            }

            return null;
        }

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
