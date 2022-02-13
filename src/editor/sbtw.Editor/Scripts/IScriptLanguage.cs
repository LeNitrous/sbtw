// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// Represents a scripting language.
    /// </summary>
    public interface IScriptLanguage : IDisposable
    {
        /// <summary>
        /// Gets scripts that can be processed by this language asynchronously.
        /// </summary>
        /// <param name="globals">The resources for use in scripts.</param>
        /// <param name="token">The cancellation token that will be used.</param>
        Task<IEnumerable<ScriptExecutionResult>> ExecuteAsync<T>(T globals, CancellationToken token = default) where T : class;

        /// <summary>
        /// Gets scripts that can be processed by this language asynchronously.
        /// </summary>
        /// <param name="token">The cancellation token that will be used.</param>
        Task<IEnumerable<ScriptExecutionResult>> ExecuteAsync(CancellationToken token = default) => ExecuteAsync<object>(null, token);
    }
}
