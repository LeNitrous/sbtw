// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Assets
{
    public abstract class Asset
    {
        [JsonIgnore]
        public Script Script { get; private set; }

        [JsonProperty]
        internal string Hash { get; private set; }

        [JsonProperty]
        internal string Path { get; private set; }

        internal bool Registered => !string.IsNullOrEmpty(Hash) && !string.IsNullOrEmpty(Path);

        internal string FullPath => Script.Storage.GetStorageForDirectory("Beatmap").GetFullPath(Path, true);

        protected virtual string CreateIdentifier() => GetType().Name;

        internal void Register(Script script)
        {
            if (!Registered)
                throw new InvalidOperationException();

            Script = script;
        }

        internal void Register(Script script, string path)
        {
            Path = path;
            Script = script;

            using var md5 = MD5.Create();

            string hash = string.Empty;
            string identifier = CreateIdentifier();

            if (!string.IsNullOrEmpty(identifier))
            {
                foreach (byte part in md5.ComputeHash(Encoding.UTF8.GetBytes($"{identifier}@{Path}")))
                    hash += part.ToString("x2");
            }

            Hash = hash;
        }

        internal void Generate() => Generate(FullPath);

        protected abstract void Generate(string path);
    }
}
