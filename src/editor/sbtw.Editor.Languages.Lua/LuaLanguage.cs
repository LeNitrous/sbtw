// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Languages.Lua.Scripts;

namespace sbtw.Editor.Languages.Lua
{
    public class LuaLanguage : Language<LuaScript>
    {
        public override string Name => @"Lua";
        public override IEnumerable<string> Extensions => new[] { ".lua" };
        protected override LuaScript CreateScript(string name, string path) => new LuaScript(name, path);
    }
}
