// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using sbtw.Editor.Scripts;
using LuaState = NLua.Lua;

namespace sbtw.Editor.Languages.Lua.Scripts
{
    public class LuaScript : Script, IDisposable
    {
        private readonly LuaState state = new LuaState();
        private readonly string code;
        private bool isDisposed;

        public LuaScript(string name, string path)
            : base(name, path)
        {
            code = File.ReadAllText(Path);
        }

        protected override void Perform()
        {
            state.DoString(@"import = function() end");
            state.DoString(code);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed && !disposing)
                return;

            state.Dispose();

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void RegisterMethod(string name, Delegate method)
            => state.RegisterFunction(name, this, method.Method);
    }
}
