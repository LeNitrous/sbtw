// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Generators
{
    public struct GeneratorResult<T, U>
    {
        public T Result { get; set; }
        public IEnumerable<string> Faulted { get; set; }
        public IEnumerable<string> Groups { get; set; }
        public IReadOnlyDictionary<IScriptedElement, U> Elements { get; set; }
        public IReadOnlyDictionary<string, IEnumerable<ScriptVariableInfo>> Variables { get; set; }
    }
}
