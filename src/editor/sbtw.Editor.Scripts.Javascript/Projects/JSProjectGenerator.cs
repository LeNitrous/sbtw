// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using osu.Framework.Platform;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts.Javascript.Resources;

namespace sbtw.Editor.Scripts.Javascript.Projects
{
    public class JSProjectGenerator : IProjectGenerator
    {
        public virtual void Generate(Storage storage)
        {
            Copy(storage, "sbtw.d.ts");
            Copy(storage, "tsconfig.json", "jsconfig.json");
            Copy(storage, "launch.json", "./.vscode/launch.json");
        }

        protected static void Copy(Storage storage, string resourceName, string targetDestination = null)
        {
            using var rStream = ResourceAssembly.Resources.GetStream(resourceName);
            using var wStream = storage.GetStream(targetDestination ?? resourceName, FileAccess.Write);
            rStream.CopyTo(wStream);
        }
    }
}
