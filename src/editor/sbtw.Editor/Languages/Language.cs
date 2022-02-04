// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GlobExpressions;
using osu.Framework.Platform;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages
{
    public abstract class Language<T> : ILanguage<T>, ILanguage
        where T : Script
    {
        public abstract string Name { get; }
        public virtual IEnumerable<string> Extensions { get; } = Array.Empty<string>();
        public virtual IEnumerable<string> Exclude { get; } = Array.Empty<string>();
        public virtual bool Enabled => true;

        protected bool IsDisposed { get; private set; }

        public IEnumerable<T> Compile(Storage storage, IEnumerable<string> ignore = null) => CompileAsync(storage, ignore).Result;

        public Task<IEnumerable<T>> CompileAsync(Storage storage, IEnumerable<string> ignore = null, CancellationToken token = default)
        {
            var toCompile = new List<T>();
            ignore ??= Array.Empty<string>();
            ignore = ignore.Concat(Exclude);

            foreach (string extension in Extensions)
            {
                foreach (string file in Directory.GetFiles(storage.GetFullPath("."), $"*{extension}", SearchOption.AllDirectories))
                {
                    if (ignore.Any(path => Glob.IsMatch(file, path, GlobOptions.CaseInsensitive)))
                        continue;

                    toCompile.Add(CreateScript(Path.GetFileNameWithoutExtension(file), file));
                }
            }

            var task = new TaskCompletionSource<IEnumerable<T>>();
            task.SetResult(toCompile);

            return task.Task;
        }

        protected abstract T CreateScript(string name, string path);
        public virtual IProjectGenerator CreateProjectGenerator() => null;
        public virtual ILanguageConfigManager CreateConfigManager() => null;

        protected virtual void Dispose(bool isDisposing)
        {
            if (IsDisposed && !isDisposing)
                return;

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        async Task<IEnumerable<Script>> ILanguage.CompileAsync(Storage storage, IEnumerable<string> ignore, CancellationToken token)
            => await CompileAsync(storage, ignore, token);

        IEnumerable<Script> ILanguage.Compile(Storage storage, IEnumerable<string> ignore)
            => Compile(storage, ignore);
    }
}
