// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;

namespace sbtw.Editor.Scripts
{
    public struct ScriptGenerationResult
    {
        public string Name { get; set; }
        public bool Faulted { get; set; }
        public IEnumerable<ScriptElementGroup> Groups { get; set; }
        public IEnumerable<ScriptVariableInfo> Variables { get; set; }
    }
}
