// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;
using osu.Game.Database;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages
{
    public interface ILanguage<T> : IDisposable
        where T : Script
    {
        Task<IEnumerable<T>> CompileAsync(Storage storage, CancellationToken token = default);
        IEnumerable<T> Compile(Storage storage);
    }

    public interface ILanguage : IDisposable
    {
        string Name { get; }
        bool Enabled { get; }
        IProjectGenerator CreateProjectGenerator();
        ILanguageConfigManager CreateConfigManager(RealmContextFactory realm);
        Task<IEnumerable<Script>> CompileAsync(Storage storage, CancellationToken token = default);
        IEnumerable<Script> Compile(Storage storage);
    }
}
