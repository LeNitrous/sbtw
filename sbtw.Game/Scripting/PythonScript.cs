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

            string script = "import clr\nclr.AddReference(\"sbtw.Common.Scripting\")\n";
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
}
