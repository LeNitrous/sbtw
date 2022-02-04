// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Assets
{
    public abstract class Asset
    {
        protected Script Script { get; private set; }

        [JsonProperty]
        private string hash;

        [JsonProperty]
        private string path;

        [JsonIgnore]
        internal string Hash => hash;

        [JsonIgnore]
        internal string Path => path;

        internal string FullPath => Script.Storage.GetStorageForDirectory("Beatmap").GetFullPath(Path, true);

        protected virtual string CreateIdentifier() => GetType().Name;

        internal void Register(Script script, string path)
        {
            Script = script;
            this.path = path;

            using var md5 = MD5.Create();

            string hash = string.Empty;
            string identifier = CreateIdentifier();

            if (!string.IsNullOrEmpty(identifier))
            {
                foreach (byte part in md5.ComputeHash(Encoding.UTF8.GetBytes($"{identifier}@{Path}")))
                    hash += part.ToString("x2");
            }

            this.hash = hash;
        }

        internal void Generate() => Generate(FullPath);

        protected abstract void Generate(string path);
    }
}
