// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Projects
{
    /// <summary>
    /// Determines that the implementer can manage scripts.
    /// </summary>
    public interface ICanProvideScripts : IDisposable
    {
        ScriptManager Scripts { get; }
    }
}
