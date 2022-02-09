// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Projects
{
    /// <summary>
    /// Denotes a project's capability of generating output.
    /// </summary>
    public interface ICanGenerate<T> : IProject, ICanProvideScripts, ICanProvideGroups
    {
        Task<T> GenerateAsync(Dictionary<string, object> resources = null, ExportTarget? target = null, bool includeHidden = true, CancellationToken token = default);
        T Generate(Dictionary<string, object> resources = null, ExportTarget? target = null, bool includeHidden = true);
    }
}
