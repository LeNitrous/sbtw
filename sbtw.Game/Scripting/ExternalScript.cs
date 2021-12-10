// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using sbtw.Common.Scripting;

namespace sbtw.Game.Scripting
{
    public abstract class ExternalScript : Script
    {
        protected internal override string Name { get; }

        protected readonly string FilePath;

        protected ExternalScript(string path)
        {
            Name = Path.GetFileNameWithoutExtension(path);
            FilePath = path;
        }
    }
}
