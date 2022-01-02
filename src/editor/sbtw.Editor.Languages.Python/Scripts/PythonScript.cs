// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Python.Runtime;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages.Python.Scripts
{
    public class PythonScript : Script, IDisposable
    {
        private readonly string code;
        private readonly Py.GILState state;
        private readonly PyModule context;
        private bool isDisposed;

        public PythonScript(string name, string path)
            : base(name, path)
        {
            code = File.ReadAllText(Path);
            state = Py.GIL();
            context = Py.CreateScope(Name);
        }

        protected override void Perform()
        {
            context.Exec(code);
        }

        protected override void RegisterMethod(string name, Delegate method)
            => context.Set(name, method);

        protected override void RegisterField(string name, object value)
            => context.Set(name, value);

        protected override void RegisterType(Type type)
            => context.Set(type.Name, type.ToPython());

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed && !disposing)
                return;

            context.Dispose();
            state.Dispose();

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
