// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;

namespace sbtw.Editor.Generators
{
    public class GeneratedGroup<T>
    {
        public string Name { get; set; }

        public readonly List<T> Elements = new List<T>();
    }
}
