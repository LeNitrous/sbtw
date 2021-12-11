// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Diagnostics;
using System.IO;
using osu.Framework;

namespace sbtw.Game.Utils
{
    public static class NetDriverHelper
    {
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
        /// <param name="exitAction">Action to invoke when process exits.</param>
        public static void Build(string path, Action exitAction = null) => start_dotnet($"build {path}", exitAction);

        /// <summary>
        /// Executes the .NET driver to clean the project output.
        /// </summary>
        /// <param name="path">The path to the project.</param>
        /// <param name="exitAction">Action to invoke when process exits.</param>
        public static void Clean(string path, Action exitAction = null) => start_dotnet($"clean {path}", exitAction);

        /// <summary>
        /// Executes the .NET driver to restore project dependencies.
        /// </summary>
        /// <param name="path">The path to the project.</param>>
        /// <param name="exitAction">Action to invoke when process exits.</param>
        public static void Restore(string path, Action exitAction = null) => start_dotnet($"restore {path}", exitAction);

        private static Process current;

        private static void start_dotnet(string args, Action exitAction = null)
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
                exitAction?.Invoke();
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

        private static string get_dotnet_path()
        {
            if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            {
                foreach (string path in PathHelper.GetEnvironmentPaths())
                {
                    if (path.Contains(@"\dotnet"))
                    {
                        if (File.Exists(Path.Combine(path, "dotnet.exe")))
                            return path;
                    }
                }
            }

            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                if (File.Exists("/usr/bin/dotnet"))
                    return "/usr/bin/dotnet";
            }

            return null;
        }
    }
}
