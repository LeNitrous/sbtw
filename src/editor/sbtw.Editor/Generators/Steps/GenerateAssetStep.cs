// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;

namespace sbtw.Editor.Generators.Steps
{
    public class GenerateAssetStep : GeneratorStep
    {
        private readonly Storage storage;

        public GenerateAssetStep(Storage storage)
        {
            this.storage = storage;
        }

        public override Task<GeneratorContext> PreProcess(GeneratorContext context, CancellationToken token)
        {
            return base.PreProcess(context, token);
        }

        public override Task<GeneratorContext> PostGenerate(GeneratorContext context, CancellationToken token)
        {
            foreach (var asset in context.Assets)
                asset.Generate(storage);

            return base.PostGenerate(context, token);
        }
    }
}
