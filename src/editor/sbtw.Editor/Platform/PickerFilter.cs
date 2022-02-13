// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;

namespace sbtw.Editor.Platform
{
    public struct PickerFilter
    {
        public IReadOnlyList<string> Files;
        public string Description;
    }
}
