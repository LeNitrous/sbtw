// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Internal;
using GlobExpressions;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    public abstract class FileBasedScriptLanguage<T> : ScriptLanguage<T>
        where T : FileBasedScript
    {
        public abstract IReadOnlyList<string> Extensions { get; }
        public virtual IReadOnlyList<string> Exclude { get; } = Array.Empty<string>();
        protected readonly List<CachedScript> Cache = new List<CachedScript>();

        protected FileBasedScriptLanguage(IProject project)
            : base(project)
        {
        }

        protected override async Task<IEnumerable<IScript>> GetScriptsAsync(CancellationToken token)
        {
            var scripts = new List<T>();
            var exclude = (Project.Exclude ?? Array.Empty<string>()).Concat(Exclude);
            var storage = (Project as ICanProvideFiles).Files;

            foreach (string extension in Extensions)
            {
                foreach (string path in Directory.GetFiles(storage.GetFullPath("."), $"*{extension}", SearchOption.AllDirectories))
                {
                    token.ThrowIfCancellationRequested();

                    if (exclude.Any(pattern => Glob.IsMatch(path, pattern, GlobOptions.CaseInsensitive)))
                        continue;

                    using var stream = storage.GetStream(path, FileAccess.Read, FileMode.Open);
                    using var md5 = MD5.Create();
                    byte[] hash = await md5.ComputeHashAsync(stream, token);

                    var cached = Cache.FirstOrDefault(c => c.Path == path);

                    if (cached.Path != null)
                    {
                        if (!cached.Hash.SequenceEqual(hash))
                        {
                            cached.Hash = hash;
                            await cached.Script.CompileAsync();
                        }
                    }
                    else
                    {
                        Cache.Add(cached = new CachedScript { Path = path, Hash = hash, Script = CreateScript(path) });
                    }

                    if (!cached.Initialized)
                    {
                        foreach ((var type, var method) in FileBasedScript.METHOD_TYPES)
                            cached.Script.RegisterDelegate(method.CreateDelegate(type, cached.Script));

                        foreach (var type in FileBasedScript.TYPES)
                            cached.Script.RegisterType(type);

                        cached.Initialized = true;
                    }

                    foreach (var member in FileBasedScript.MEMBERS)
                        cached.Script.RegisterMember(member.Name, member.GetMemberValue(cached.Script.Resources));

                    scripts.Add(cached.Script);
                }
            }

            foreach (var cached in Cache)
            {
                if (File.Exists(cached.Path))
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
            public bool Initialized;
            public T Script;
        }
    }
}
