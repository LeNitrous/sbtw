// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using osu.Framework.Platform;
using sbtw.Editor.Languages.Javascript.Resources;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Languages.Javascript.Projects
{
    public class JavascriptProjectGenerator : IProjectGenerator
    {
        public virtual void Generate(Storage storage)
        {
            copy(storage, "sbtw.d.ts");
            copy(storage, "tsconfig.json", "tsconfig.json");
            copy(storage, "launch.json", "./.vscode/launch.json");
        }

        private static void copy(Storage storage, string resourceName, string targetDestination = null)
        {
            using var rStream = ResourceAssembly.GetStream(resourceName);
            using var wStream = storage.GetStream(targetDestination ?? resourceName, FileAccess.Write);
            rStream.CopyTo(wStream);
        }
    }
}
