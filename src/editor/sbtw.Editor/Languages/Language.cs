// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;
using osu.Game.Database;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages
{
    public abstract class Language<T> : ILanguage<T>, ILanguage
        where T : Script
    {
        public abstract string Name { get; }
        public virtual IEnumerable<string> Extensions => Array.Empty<string>();

        protected bool IsDisposed { get; private set; }
        protected IReadOnlyList<T> Cache => cache.Select(c => c.Script).ToList();

        private readonly List<ScriptCompileInfo> cache = new List<ScriptCompileInfo>();

        public IEnumerable<T> Compile(Storage storage) => CompileAsync(storage).Result;

        public Task<IEnumerable<T>> CompileAsync(Storage storage, CancellationToken token = default)
        {
            var scriptInfos = new List<ScriptCompileInfo>();

            foreach (string extension in Extensions)
            {
                foreach (string path in storage.GetFiles(".", $"*.{extension}"))
                {
                    var scriptInfo = cache.FirstOrDefault(s => s.Path == path);

                    if (scriptInfo != null && scriptInfo.LastCompileTime != DateTimeOffset.Now)
                    {
                        scriptInfos.Add(scriptInfo);
                        scriptInfo.LastCompileTime = DateTimeOffset.Now;
                    }
                    else
                    {
                        scriptInfos.Add(new ScriptCompileInfo(CreateScript(Path.GetFileNameWithoutExtension(path), path), path));
                    }
                }
            }

            foreach (var scriptInfo in scriptInfos)
                scriptInfo.Script.Compile();

            cache.Clear();
            cache.AddRange(scriptInfos);

            var task = new TaskCompletionSource<IEnumerable<T>>();
            task.SetResult(scriptInfos.Select(s => s.Script));

            return task.Task;
        }

        protected virtual void Clear() => cache.Clear();

        protected abstract T CreateScript(string name, string path);
        public virtual IProjectGenerator CreateProjectGenerator() => null;
        public virtual ILanguageConfigManager CreateConfigManager(RealmContextFactory realm) => null;

        protected virtual void Dispose(bool isDisposing)
        {
            if (IsDisposed && !isDisposing)
                return;

            Clear();

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        Task<IEnumerable<Script>> ILanguage.CompileAsync(Storage storage, CancellationToken token)
            => CompileAsync(storage, token) as Task<IEnumerable<Script>>;

        IEnumerable<Script> ILanguage.Compile(Storage storage)
            => Compile(storage);

        private class ScriptCompileInfo
        {
            public T Script { get; set; }
            public string Path { get; set; }
            public DateTimeOffset LastCompileTime { get; set; }

            public ScriptCompileInfo(T script, string path)
            {
                Path = path;
                Script = script;
                LastCompileTime = DateTimeOffset.Now;
            }
        }
    }
}
