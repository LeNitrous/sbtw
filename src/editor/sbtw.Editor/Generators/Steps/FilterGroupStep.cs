// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Generators.Steps
{
    public class FilterGroupStep : GeneratorStep
    {
        private readonly ExportTarget? target;
        private readonly bool includeHidden;

        public FilterGroupStep(ExportTarget? target = null, bool includeHidden = true)
        {
            this.target = target;
            this.includeHidden = includeHidden;
        }

        public override Task<GeneratorContext> PostProcess(GeneratorContext context, CancellationToken token)
        {
            if (target.HasValue && target.Value != ExportTarget.None)
                context.Groups = context.Groups.Where(g => g.Target.Value == target.Value);

            if (!includeHidden)
                context.Groups = context.Groups.Where(g => g.Visible.Value);

            return Task.FromResult(context);
        }
    }
}
