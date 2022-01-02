// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Editor.Scripts
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class VisibleAttribute : Attribute
    {
    }
}
