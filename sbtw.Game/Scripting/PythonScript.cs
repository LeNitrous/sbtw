// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Python.Runtime;
using sbtw.Common.Scripting;

namespace sbtw.Game.Scripting
{
    public class PythonScript : ExternalScript
    {
        public PythonScript(string path)
            : base(path)
        {
        }

        public override void Generate()
        {
            using var _ = Py.GIL();
            using var ctx = Py.CreateScope("ctx");
            ctx.Set("SetVideo", new Action<string, int>(SetVideo));
            ctx.Set("GetGroup", new Func<string, ScriptElementGroup>(GetGroup));

            using var stream = File.OpenRead(FilePath);
            using var reader = new StreamReader(stream);

            string script = @"import clr
clr.AddReference(""sbtw.Common.Scripting"")
clr.AddReference(""System.Numerics"")

from System.Numerics import Vector2
from sbtw.Common.Scripting import *
";

            script += reader.ReadToEnd();

            try
            {
                ctx.Exec(script);
            }
            catch (PythonException e)
            {
                throw new PythonExecutionException(FilePath, e);
            }
        }
    }

    public class PythonExecutionException : Exception
    {
        public PythonExecutionException(string file, PythonException e)
            : base(get_message(file, e.Message), e)
        {
        }

        private static string get_message(string file, string message)
        {
            return $"File \"{file}\" has errors:\n{message.Replace("\n", "\n\t")}";
        }
    }
}
