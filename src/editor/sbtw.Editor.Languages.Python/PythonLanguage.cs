// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using osu.Framework;
using Python.Runtime;
using sbtw.Editor.Languages.Python.Scripts;
using PythonRuntime = Python.Runtime.Runtime;

namespace sbtw.Editor.Languages.Python
{
    public class PythonLanguage : Language<PythonScript>
    {
        public static readonly string PYTHON_RUNTIME_PATH;

        static PythonLanguage()
        {
            if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            {
                foreach (string path in Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User).Split(";"))
                {
                    if (!path.Contains("Python"))
                        continue;

                    string pythonPath = path.Replace("Scripts\\", string.Empty);
                    pythonPath += $"{new DirectoryInfo(pythonPath).Name.ToLowerInvariant()}.dll";

                    if (!File.Exists(pythonPath))
                        continue;

                    PYTHON_RUNTIME_PATH = pythonPath;
                    break;
                }
            }

            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                foreach (string file in Directory.GetFiles("/usr/lib/"))
                {
                    if (!file.Contains("libpython"))
                        continue;

                    PYTHON_RUNTIME_PATH = $"/usr/lib/{file}";
                    break;
                }
            }
        }

        public override string Name => @"Python";
        public override IEnumerable<string> Extensions => new[] { "py" };
        public override bool Enabled => !string.IsNullOrEmpty(PYTHON_RUNTIME_PATH);

        public PythonLanguage()
        {
            PythonRuntime.PythonDLL = PYTHON_RUNTIME_PATH;

            if (Enabled)
                PythonEngine.Initialize();
        }

        protected override PythonScript CreateScript(string name, string path) => new PythonScript(name, path);

        protected override void Dispose(bool isDisposing)
        {
            if (Enabled)
                PythonEngine.Shutdown();

            base.Dispose(isDisposing);
        }
    }
}
