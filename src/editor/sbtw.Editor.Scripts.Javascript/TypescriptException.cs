// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Editor.Scripts.Javascript
{
    public class TypescriptException : Exception
    {
        public TypescriptException(string message)
            : base(message)
        {
        }
    }
}
