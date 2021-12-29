// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using NUnit.Framework;
using sbtw.Editor.Languages.Javascript.Tests;
using sbtw.Editor.Languages.Lua.Scripts;
using sbtw.Editor.Languages.Tests;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages.Lua.Tests
{
    [TestFixture]
    public class LuaScriptTest : ScriptTest
    {
        protected override string Extension => "lua";

        protected override Script CreateScript(string path) => new LuaScript(string.Empty, path);
        protected override Stream GetStream(string path) => LuaTestResources.GetStream(path);
    }
}
