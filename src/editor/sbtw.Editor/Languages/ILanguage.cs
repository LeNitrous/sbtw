// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages
{
    public interface ILanguage<T> : IDisposable
        where T : Script
    {
        Task<IEnumerable<T>> CompileAsync(Storage storage, IEnumerable<string> ignore = null, CancellationToken token = default);
        IEnumerable<T> Compile(Storage storage, IEnumerable<string> ignore = null);
    }

    public interface ILanguage : IDisposable
    {
        string Name { get; }
        bool Enabled { get; }
        IProjectGenerator CreateProjectGenerator();
        ILanguageConfigManager CreateConfigManager();
        Task<IEnumerable<Script>> CompileAsync(Storage storage, IEnumerable<string> ignore = null, CancellationToken token = default);
        IEnumerable<Script> Compile(Storage storage, IEnumerable<string> ignore = null);
    }
}
