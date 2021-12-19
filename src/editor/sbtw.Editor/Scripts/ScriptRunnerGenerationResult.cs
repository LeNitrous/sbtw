// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;

namespace sbtw.Editor.Scripts
{
    public struct ScriptRunnerGenerationResult<TResult, TGenerated>
        where TResult : new()
    {
        public TResult Result { get; set; }
        public IEnumerable<string> Groups { get; set; }
        public IReadOnlyDictionary<IScriptedElement, TGenerated> Map { get; set; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> Variables { get; set; }
    }
}
