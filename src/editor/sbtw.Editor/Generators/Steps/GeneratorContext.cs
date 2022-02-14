// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using sbtw.Editor.Scripts.Assets;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Generators.Steps
{
    public class GeneratorContext
    {
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Asset> Assets { get; set; }
    }
}
