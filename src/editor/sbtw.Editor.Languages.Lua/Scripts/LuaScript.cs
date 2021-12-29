// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.Linq;
using sbtw.Editor.Scripts;
using LuaState = NLua.Lua;

namespace sbtw.Editor.Languages.Lua.Scripts
{
    public class LuaScript : Script, IDisposable
    {
        public override string Name { get; }

        private readonly LuaState state = new LuaState();
        private readonly string code;
        private bool isDisposed;

        public LuaScript(string name, string path)
        {
            Name = name;
            code = File.ReadAllText(path);
            state.RegisterFunction("GetValue", this, GetType().GetMethods().FirstOrDefault(m => m.ReturnType == typeof(object)));
            state.RegisterFunction("SetValue", this, GetType().GetMethods().FirstOrDefault(m => m.ReturnType == typeof(object)));
            state.RegisterFunction("SetVideo", this, GetType().GetMethod("SetVideo"));
            state.RegisterFunction("GetGroup", this, GetType().GetMethod("GetGroup"));
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
    }
}
