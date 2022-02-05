// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using GlobExpressions;
using osu.Framework.Platform;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Languages
{
    public abstract class Language : ILanguage
    {
        public abstract string Name { get; }
        public virtual IEnumerable<string> Extensions { get; } = Array.Empty<string>();
        public virtual IEnumerable<string> Exclude { get; } = Array.Empty<string>();
        protected bool IsDisposed { get; private set; }
        protected readonly List<CachedScript> Cache = new List<CachedScript>();

        public void Reset() => Cache.Clear();

        public virtual string GetExceptionMessage(Exception exception) => exception.Message;
        public virtual IProjectGenerator CreateProjectGenerator() => null;
        public virtual ILanguageConfigManager CreateConfigManager() => null;
        public abstract Task<IEnumerable<Script>> CompileAsync(Storage storage, IEnumerable<string> ignore = null, CancellationToken token = default);
        public abstract IEnumerable<Script> Compile(Storage storage, IEnumerable<string> ignore = null);

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

        public class CachedScript
        {
            public string Path;
            public byte[] Hash;
            public Script Script;
        }
    }

    public abstract class Language<T> : Language, ILanguage<T>
        where T : Script
    {
        IEnumerable<T> ILanguage<T>.Compile(Storage storage, IEnumerable<string> ignore) => (this as ILanguage<T>).CompileAsync(storage, ignore).Result;

        async Task<IEnumerable<T>> ILanguage<T>.CompileAsync(Storage storage, IEnumerable<string> ignore, CancellationToken token)
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

                    using var stream = File.OpenRead(file);
                    using var md5 = MD5.Create();
                    byte[] hash = await md5.ComputeHashAsync(stream, token);

                    var cached = Cache.FirstOrDefault(c => c.Path == file);

                    if (cached?.Path == file)
                    {
                        if (!cached.Hash.SequenceEqual(hash))
                        {
                            cached.Hash = hash;
                            cached.Script.Dispose();
                            cached.Script = CreateScript(Path.GetFileNameWithoutExtension(file), file);
                        }

                        toCompile.Add(cached.Script as T);
                    }
                    else
                    {
                        var script = CreateScript(Path.GetFileNameWithoutExtension(file), file);
                        Cache.Add(new CachedScript { Path = file, Hash = hash, Script = script });
                        toCompile.Add(script);
                    }
                }
            }

            foreach (var cached in Cache)
            {
                if (!File.Exists(cached.Path))
                {
                    cached.Script.Dispose();
                    Cache.Remove(cached);
                }
            }

            return toCompile;
        }

        protected abstract T CreateScript(string name, string path);

        public override async Task<IEnumerable<Script>> CompileAsync(Storage storage, IEnumerable<string> ignore = null, CancellationToken token = default)
            => await (this as ILanguage<T>).CompileAsync(storage, ignore, token);

        public override IEnumerable<Script> Compile(Storage storage, IEnumerable<string> ignore = null)
            => (this as ILanguage<T>).Compile(storage, ignore);
    }
}
