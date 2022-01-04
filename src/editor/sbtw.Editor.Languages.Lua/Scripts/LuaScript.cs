// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.Text;
using sbtw.Editor.Scripts;
using LuaState = NLua.Lua;

namespace sbtw.Editor.Languages.Lua.Scripts
{
    public class LuaScript : Script
    {
        private LuaState lua = new LuaState { State = { Encoding = Encoding.UTF8 } };

        public LuaScript(string name, string path)
            : base(name, path)
        {
        }

        protected override void Perform()
        {
            lua.DoString(@"import = function() end");
            lua.DoString(File.ReadAllText(Path));
        }

        protected override void RegisterMethod(string name, Delegate method)
            => lua.RegisterFunction(name, this, method.Method);

        protected override void RegisterField(string name, object value)
            => lua[name] = value;

        protected override void RegisterType(Type type)
            => lua.DoString($"{type.Name} = luanet.import_type('{type.Namespace}.{type.Name}')");

        protected override void Dispose(bool disposing)
        {
            lua?.Dispose();
            lua = null;
            base.Dispose(disposing);
        }
    }
}
