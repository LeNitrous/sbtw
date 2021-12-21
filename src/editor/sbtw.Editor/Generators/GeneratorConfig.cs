// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Generators
{
    public struct GeneratorConfig
    {
        public IEnumerable<Script> Scripts { get; set; }
        public IEnumerable<string> Ordering { get; set; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> Variables { get; set; }
    }
}
