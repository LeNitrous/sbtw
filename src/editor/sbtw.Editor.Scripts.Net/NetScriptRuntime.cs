// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts.Net
{
    public class NetScriptRuntime : ScriptRuntime
    {
        public override Task<IEnumerable<Script>> PrepareAsync(Storage storage, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
