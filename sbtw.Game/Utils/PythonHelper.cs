// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using osu.Framework;
using Python.Runtime;

namespace sbtw.Game.Utils
{
    public static class PythonHelper
    {
        public static bool HAS_PYTHON => !string.IsNullOrEmpty(PYTHON_PATH);

        public static string PYTHON_PATH => get_python_library();

        static PythonHelper()
        {
            Runtime.PythonDLL = get_python_library();
        }

        private static string get_python_library()
        {
            if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            {
                string pythonPath = string.Empty;

                foreach (string path in PathHelper.GetEnvironmentPaths())
                {
                    if (path.Contains("Python"))
                    {
                        pythonPath = path.Replace("Scripts\\", string.Empty);
                        pythonPath += $"{new DirectoryInfo(pythonPath).Name.ToLowerInvariant()}.dll";

                        if (File.Exists(pythonPath))
                            break;
                    }
                }

                return pythonPath;
            }

            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                foreach (string file in Directory.GetFiles("/usr/lib/"))
                {
                    if (file.Contains("libpython"))
                        return $"/usr/lib/{file}";
                }
            }

            return string.Empty;
        }
    }
}
