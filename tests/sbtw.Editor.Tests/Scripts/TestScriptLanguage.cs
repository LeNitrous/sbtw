// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestScriptLanguage : ScriptLanguage
    {
        public IEnumerable<IScript> Scripts { get; set; } = Enumerable.Empty<IScript>();

        public TestScriptLanguage(IProject project)
            : base(project)
        {
        }

        protected override Task<IEnumerable<IScript>> GetScriptsAsync(CancellationToken token = default)
            => Task.FromResult(Scripts);
    }
}
