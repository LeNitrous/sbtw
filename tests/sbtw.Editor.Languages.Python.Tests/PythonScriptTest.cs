// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using NUnit.Framework;
using Python.Runtime;
using sbtw.Editor.Languages.Python.Scripts;
using sbtw.Editor.Languages.Tests;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages.Python.Tests
{
    [TestFixture]
    public class PythonScriptTest : ScriptTest
    {
        protected override string Extension => "py";

        [OneTimeSetUp]
        public void PythonSetUp()
        {
            Runtime.PythonDLL = PythonLanguage.PYTHON_RUNTIME_PATH;
            PythonEngine.Initialize();
        }

        [OneTimeTearDown]
        public void PythonTearDown()
        {
            PythonEngine.Shutdown();
        }

        protected override Script CreateScript(string path) => new PythonScript(string.Empty, path);
        protected override Stream GetStream(string path) => PythonTestResources.GetStream(path);
    }
}
