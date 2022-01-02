// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.Text;
using sbtw.Editor.Scripts;
using LuaState = NLua.Lua;

namespace sbtw.Editor.Languages.Lua.Scripts
{
    public class LuaScript : Script, IDisposable
    {
        private readonly LuaState lua = new LuaState { State = { Encoding = Encoding.UTF8 } };
        private readonly string code;
        private bool isDisposed;

        public LuaScript(string name, string path)
            : base(name, path)
        {
            code = File.ReadAllText(Path);
        }

        protected override void Perform()
        {
            lua.DoString(@"import = function() end");
            lua.DoString(code);
        }

        protected override void RegisterMethod(string name, Delegate method)
            => lua.RegisterFunction(name, this, method.Method);

        protected override void RegisterField(string name, object value)
            => lua[name] = value;

        protected override void RegisterType(Type type)
            => lua.DoString($"{type.Name} = luanet.import_type('{type.Namespace}.{type.Name}')");

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed && !disposing)
                return;

            lua.Dispose();

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
