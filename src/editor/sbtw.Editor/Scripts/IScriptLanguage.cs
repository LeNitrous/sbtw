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
        /// The name of the scripting language.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Paths to files that should be excluded during search at a script language level. Glob patterns are accepted.
        /// </summary>
        IReadOnlyList<string> Exclude { get; }

        /// <summary>
        /// File extensions this scripting language can support.
        /// </summary>
        IReadOnlyList<string> Extensions { get; }

        /// <summary>
        /// Gets the exception message for this scripting language.
        /// </summary>
        /// <param name="exception">The exception raised.</param>
        /// <returns>The exception message.</returns>
        string GetExceptionMessage(Exception exception);

        /// <summary>
        /// Gets scripts that can be processed by this language.
        /// </summary>
        /// <param name="resources">The resources for use in scripts.</param>
        /// <returns></returns>
        IEnumerable<IScript> GetScripts(ScriptResources resources);

        /// <summary>
        /// Gets scripts that can be processed by this language asynchronously.
        /// </summary>
        /// <param name="resources">The resources for use in scripts.</param>
        /// <param name="token">The cancellation token that will be used.</param>
        /// <returns>A collection of scripts.</returns>
        Task<IEnumerable<IScript>> GetScriptsAsync(ScriptResources resources, CancellationToken token = default);
    }

    /// <summary>
    /// Represents a scripting language of a given type of script.
    /// </summary>
    /// <typeparam name="T">The script type this language can enact on.</typeparam>
    public interface IScriptLanguage<T>
        where T : IScript
    {
        /// <summary>
        /// Gets scripts that can be processed by this language.
        /// </summary>
        /// <param name="resources">The resources for use in scripts.</param>
        /// <returns>A collection of scripts.</returns>
        IEnumerable<T> GetScripts(ScriptResources resources);

        /// <summary>
        /// Gets scripts that can be processed by this language asynchronously.
        /// </summary>
        /// <param name="resources">The resources for use in scripts.</param>
        /// <param name="token">The cancellation token that will be used.</param>
        /// <returns>A collection of scripts.</returns>
        Task<IEnumerable<T>> GetScriptsAsync(ScriptResources resources, CancellationToken token = default);
    }
}
