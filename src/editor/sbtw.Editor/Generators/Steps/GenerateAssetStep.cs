// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;
using sbtw.Editor.Scripts.Assets;

namespace sbtw.Editor.Generators.Steps
{
    public class GenerateAssetStep : GeneratorStep
    {
        private readonly Storage storage;
        private IReadOnlyList<Asset> before;

        public GenerateAssetStep(Storage storage)
        {
            this.storage = storage;
        }

        public override Task<GeneratorContext> PreProcess(GeneratorContext context, CancellationToken token)
        {
            before = context.Assets.ToArray();
            return base.PreProcess(context, token);
        }

        public override Task<GeneratorContext> PostGenerate(GeneratorContext context, CancellationToken token)
        {
            var after = context.Assets.ToArray();

            var added = after.Except(before, EqualityComparer<Asset>.Default);
            var removed = before.Except(added, EqualityComparer<Asset>.Default);

            foreach (var asset in added)
                asset.Generate(storage);

            foreach (var asset in removed)
                storage.Delete(asset.Path);

            return base.PostGenerate(context, token);
        }
    }
}
