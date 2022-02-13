// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Editor.Scripts
{
    public struct ScriptExecutionResult
    {
        public IScript Script;
        public Exception Exception;
        public bool Faulted => Exception != null;
    }
}
