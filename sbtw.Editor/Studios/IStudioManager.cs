// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Bindables;

namespace sbtw.Editor.Studios
{
    public interface IStudioManager
    {
        public IEnumerable<Studio> Studios { get; }

        public Bindable<Studio> Current { get; }
    }
}
