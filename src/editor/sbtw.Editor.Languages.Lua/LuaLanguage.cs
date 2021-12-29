// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Languages.Lua.Scripts;

namespace sbtw.Editor.Languages.Lua
{
    public class LuaLanguage : Language<LuaScript>
    {
        public override string Name => @"Lua";

        protected override LuaScript CreateScript(string name, string path) => new LuaScript(name, path);

        protected override void Clear()
        {
            foreach (var script in Cache)
                script.Dispose();

            base.Clear();
        }
    }
}
