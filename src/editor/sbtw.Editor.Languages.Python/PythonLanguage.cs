// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using Microsoft.Scripting.Hosting;
using sbtw.Editor.Languages.Python.Scripts;
using Py = IronPython.Hosting.Python;

namespace sbtw.Editor.Languages.Python
{
    public class PythonLanguage : Language<PythonScript>
    {
        public override string Name => @"Python";
        public override IEnumerable<string> Extensions => new[] { ".py" };
        private readonly ScriptEngine engine = Py.CreateEngine(new Dictionary<string, object> { { "Debug", true } });

        public PythonLanguage()
        {
        }

        protected override PythonScript CreateScript(string name, string path) => new PythonScript(engine.CreateScope(), name, path);
    }
}
