// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts.BuiltIns;

namespace sbtw.Editor.Scripts
{
    public class BuiltinScriptLanguage : ScriptLanguage
    {
        private readonly IEnumerable<BuiltInScript> scripts = new[]
        {
            new BeatmapBackgroundScript(),
        };

        public BuiltinScriptLanguage(IProject project)
            : base(project)
        {
        }

        protected override Task<IEnumerable<IScript>> GetScriptsAsync(CancellationToken token = default)
        {
            foreach (var script in scripts)
                script.Reset();

            return Task.FromResult<IEnumerable<IScript>>(scripts);
        }
    }
}
