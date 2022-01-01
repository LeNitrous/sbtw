// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Editor.Scripts
{
    public class ScriptExecutionException : Exception
    {
        public ScriptExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
