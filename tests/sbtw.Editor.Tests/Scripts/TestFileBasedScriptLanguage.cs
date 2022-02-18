// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Platform;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestFileBasedScriptLanguage : FileBasedScriptLanguage<TestFileBasedScript>
    {
        public override IReadOnlyList<string> Extensions { get; } = new[] { ".txt" };
        public new IReadOnlyList<TestFileBasedScript> Cache => base.Cache.Select(c => c.Script).ToArray();

        public TestFileBasedScriptLanguage(Storage storage)
            : base(storage)
        {
        }

        protected override TestFileBasedScript CreateScript(string path)
            => new TestFileBasedScript(path);
    }
}
