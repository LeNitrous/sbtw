// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using Microsoft.Scripting.Hosting;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages.Python.Scripts
{
    public class PythonScript : Script
    {
        private readonly ScriptScope scope;
        private readonly ScriptEngine engine;

        public PythonScript(ScriptScope scope, string name, string path)
            : base(name, path)
        {
            this.scope = scope;
            engine = scope.Engine;
        }

        protected override void Perform()
        {
            engine.SetSearchPaths(new[] { System.IO.Path.GetDirectoryName(Path) });
            engine.ExecuteFile(Path, scope);
        }

        protected override void RegisterMethod(string name, Delegate method)
            => scope.SetVariable(name, method);

        protected override void RegisterField(string name, object value)
            => scope.SetVariable(name, value);

        protected override void RegisterType(Type type)
        {
            engine.Execute(@$"
import clr
clr.AddReference(""{type.Namespace}"")
from {type.Namespace} import {type.Name}
", scope);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
