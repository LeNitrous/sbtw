// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Python.Runtime;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages.Python.Scripts
{
    public class PythonScript : Script
    {
        public override string Name { get; }

        private readonly string code;

        public PythonScript(string name, string path)
        {
            Name = name;
            code = File.ReadAllText(path);
        }

        protected override void Perform()
        {
            using var _ = Py.GIL();
            using var ctx = Py.CreateScope(Name);
            ctx.Set("SetValue", new Func<string, object, object>(SetValue));
            ctx.Set("GetValue", new Func<string, object>(GetValue));
            ctx.Set("SetVideo", new Action<string, int>(SetVideo));
            ctx.Set("GetGroup", new Func<string, ScriptElementGroup>(GetGroup));
            ctx.Exec(code);
        }
    }
}
