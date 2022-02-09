// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// Denotes that a method should be invocable in an external script context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ScriptVisibleAttribute : Attribute
    {
    }
}
