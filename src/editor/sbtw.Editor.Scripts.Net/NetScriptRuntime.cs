// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Platform;
using sbtw.Editor.Configuration;

namespace sbtw.Editor.Scripts.Net
{
    public class NetScriptRuntime : ScriptRuntime
    {
        public NetScriptRuntime(EditorConfigManager config)
            : base(config)
        {
        }

        public override IEnumerable<CompilableScript> Prepare(Storage storage)
        {
            throw new System.NotImplementedException();
        }
    }
}
