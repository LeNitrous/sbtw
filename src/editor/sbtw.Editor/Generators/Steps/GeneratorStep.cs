// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Generators.Steps
{
    public abstract class GeneratorStep
    {
        public virtual Task<GeneratorContext> PreProcess(GeneratorContext context, CancellationToken token)
        {
            return Task.FromResult(context);
        }

        public virtual Task<GeneratorContext> PreGenerate(GeneratorContext context, CancellationToken token)
        {
            return Task.FromResult(context);
        }

        public virtual Task<GeneratorContext> PostProcess(GeneratorContext context, CancellationToken token)
        {
            return Task.FromResult(context);
        }

        public virtual Task<GeneratorContext> PostGenerate(GeneratorContext context, CancellationToken token)
        {
            return Task.FromResult(context);
        }
    }
}
