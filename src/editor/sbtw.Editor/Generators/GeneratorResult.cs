// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Generators
{
    public class GeneratorResult
    {
        public IEnumerable<ScriptExecutionResult> Scripts { get; set; }
    }

    public class GeneratorResult<T> : GeneratorResult
    {
        public T Result { get; set; }
    }
}
