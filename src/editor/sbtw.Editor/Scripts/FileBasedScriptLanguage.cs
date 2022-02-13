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
using sbtw.Editor.Extensions;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// A scripting language that relies on external scripts.
    /// </summary>
    public abstract class FileBasedScriptLanguage : ScriptLanguage
    {
        /// <summary>
        /// An array of extensions (including the period at the start)
        /// that will be used in searching for files in a given storage.
        /// </summary>
        public abstract IReadOnlyList<string> Extensions { get; }

        /// <summary>
        /// An array of of file names and glob patterns that will be excluded from file search.
        /// </summary>
        public virtual IReadOnlyList<string> Exclude { get; } = Array.Empty<string>();

        protected FileBasedScriptLanguage(IProject project)
            : base(project)
        {
        }
    }

    /// <summary>
    /// A scripting language that relies on external scripts.
    /// </summary>
    public abstract class FileBasedScriptLanguage<T> : FileBasedScriptLanguage
        where T : FileBasedScript
    {
        protected readonly List<CachedScript> Cache = new List<CachedScript>();
        private readonly Storage storage;

        protected FileBasedScriptLanguage(IProject project)
            : base(project)
        {
            storage = (Project as ICanProvideFiles)?.Files ?? throw new NotSupportedException(@"Project does not provide files.");
        }

        protected sealed override async Task<IEnumerable<IScript>> GetScriptsAsync(CancellationToken token = default)
        {
            var scripts = new List<T>();

            foreach (string extension in Extensions)
            {
                foreach (string path in storage.GetFiles(".", $"*{extension}", SearchOption.AllDirectories))
                {
                    token.ThrowIfCancellationRequested();

                    if (Exclude.Any(pattern => Glob.IsMatch(path, pattern, GlobOptions.CaseInsensitive)))
                        continue;

                    using var stream = storage.GetStream(path, FileAccess.Read, FileMode.Open);
                    using var md5 = MD5.Create();
                    byte[] hash = await md5.ComputeHashAsync(stream, token);

                    var cached = Cache.FirstOrDefault(c => c.Path == path);

                    if (cached != null)
                    {
                        if (!cached.Hash.SequenceEqual(hash))
                        {
                            cached.Hash = hash;
                            await cached.Script.CompileAsync(token);
                        }
                    }
                    else
                    {
                        Cache.Add(cached = new CachedScript { Path = path, Hash = hash, Script = CreateScript(storage.GetFullPath(path)) });
                        await cached.Script.CompileAsync(token);
                    }

                    scripts.Add(cached.Script);
                }
            }

            foreach (var cached in Cache)
            {
                if (storage.Exists(cached.Path))
                    continue;

                cached.Script.Dispose();
                Cache.Remove(cached);
            }

            return scripts;
        }

        protected abstract T CreateScript(string path);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach (var cached in Cache)
                cached.Script.Dispose();

            Cache.Clear();
        }

        protected class CachedScript
        {
            public string Path;
            public byte[] Hash;
            public T Script;
        }
    }
}
