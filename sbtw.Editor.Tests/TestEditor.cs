// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Scripts;
using sbtw.Editor.Studios;
using sbtw.Editor.Tests.Scripts;
using sbtw.Editor.Tests.Studios;

namespace sbtw.Editor.Tests
{
    public class TestEditor : EditorBase
    {
        protected override IScriptRuntime CreateScriptRuntime() => new TestScriptRuntime();
        protected override IStudioManager CreateStudioManager() => new TestStudioManager();
    }
}
