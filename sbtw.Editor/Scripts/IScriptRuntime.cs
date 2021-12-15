// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;

namespace sbtw.Editor.Scripts
{
    public interface IScriptRuntime
    {
        void Clear();

        IEnumerable<Script> Compile();
    }
}
