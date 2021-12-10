// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using osu.Framework;

namespace sbtw.Game.Utils
{
    public static class CodeHelper
    {
        /// <summary>
        /// Returns an enumerable list of paths to installed code editors based on the "PATH" environment variable.
        /// </summary>
        public static readonly Dictionary<string, string> EDITORS = get_installed_code_editor_paths();

        private static Dictionary<string, string> get_installed_code_editor_paths()
        {
            var found = new Dictionary<string, string>();

            if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            {
                string[] known = new[]
                {
                    "Microsoft VS Code",
                    "Microsoft VS Code Insiders",
                };

                foreach (string path in PathHelper.GetEnvironmentPaths())
                {
                    foreach (string editor in known)
                    {
                        if (path.Contains($@"\{editor}\") && !found.ContainsKey(path))
                        {
                            var editorPath = Path.Combine(path, "code");
                            if (File.Exists(editorPath))
                                found.Add(editorPath, editor);
                        }
                    }
                }
            }

            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                string[] known = new[]
                {
                    "code",
                    "code_insiders"
                };

                foreach (string editor in known)
                {
                    string path = $@"/usr/bin/{editor}";
                    if (File.Exists(path))
                        found.Add(path, editor);
                }
            }

            return found;
        }
    }
}
