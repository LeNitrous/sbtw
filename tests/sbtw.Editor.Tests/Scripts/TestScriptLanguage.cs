// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Tests.Scripts;

namespace sbtw.Editor.Tests
{
    public class TestScriptLanguage : ScriptLanguage<TestScript>
    {
        public override string Name => @"Test";
        public IReadOnlyList<TestScript> Scripts { get; set; }

        public TestScriptLanguage(IProject project)
            : base(project)
        {
        }

        protected override Task<IEnumerable<IScript>> GetScriptsAsync(CancellationToken token = default)
            => Task.FromResult<IEnumerable<IScript>>(Scripts);
    }
}
