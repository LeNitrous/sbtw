// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Assets;

namespace sbtw.Editor.Scripts
{
    public struct ScriptGenerationResult
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Faulted { get; set; }
        public IEnumerable<Asset> Assets { get; set; }
        public IEnumerable<ScriptElementGroup> Groups { get; set; }
    }
}
