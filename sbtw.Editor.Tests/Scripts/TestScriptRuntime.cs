// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestScriptRuntime : IScriptRuntime
    {
        private readonly List<Script> cached = new List<Script>();

        public void Clear() => cached.Clear();
        public void Add(Script script) => cached.Add(script);
        public void AddRange(IEnumerable<Script> scripts) => cached.AddRange(scripts);
        public IEnumerable<Script> Compile() => cached;
    }
}
