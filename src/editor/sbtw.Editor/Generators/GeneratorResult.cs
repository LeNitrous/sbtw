// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Assets;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Generators
{
    public struct GeneratorResult<T, U>
    {
        public T Result { get; set; }
        public IEnumerable<Asset> Assets { get; set; }
        public IEnumerable<ScriptGenerationResult> Scripts { get; set; }
        public IReadOnlyDictionary<string, List<U>> Groups { get; set; }
    }
}
