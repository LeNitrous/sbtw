// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;
using sbtw.Editor.Languages.Python.Scripts;
using sbtw.Editor.Languages.Tests;
using sbtw.Editor.Scripts;
using Py = IronPython.Hosting.Python;

namespace sbtw.Editor.Languages.Python.Tests
{
    [TestFixture]
    public class PythonScriptTest : ScriptTest
    {
        private readonly ScriptEngine engine = Py.CreateEngine(new Dictionary<string, object> { { "Debug", true } });

        protected override string Extension => "py";
        protected override Script CreateScript(string path) => new PythonScript(engine.CreateScope(), string.Empty, path);
        protected override Stream GetStream(string path) => PythonTestResources.GetStream(path);
    }
}
