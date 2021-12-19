// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;

namespace sbtw.Editor.Scripts.Javascript.Projects
{
    public class TSProjectGenerator : JSProjectGenerator
    {
        public override void Generate(Storage storage)
        {
            Copy(storage, "sbtw.d.ts");
            Copy(storage, "tsconfig.json");
            Copy(storage, "launch.json", "./.vscode/launch.json");
        }
    }
}
