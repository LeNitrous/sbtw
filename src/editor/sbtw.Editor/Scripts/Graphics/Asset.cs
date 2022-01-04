// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Security.Cryptography;
using System.Text;

namespace sbtw.Editor.Scripts.Graphics
{
    public abstract class Asset
    {
        protected Script Script { get; private set; }
        internal string Hash { get; private set; }
        internal string Path { get; private set; }
        internal string FullPath => Script.Storage.GetStorageForDirectory("Beatmap").GetFullPath(Path, true);

        protected virtual string CreateIdentifier() => null;

        internal void Register(Script script, string path)
        {
            Script = script;
            Path = path;

            using var md5 = MD5.Create();

            string hash = string.Empty;
            string identifier = CreateIdentifier();

            if (!string.IsNullOrEmpty(identifier))
            {
                foreach (byte part in md5.ComputeHash(Encoding.UTF8.GetBytes($"{identifier}@{FullPath}")))
                    hash += part.ToString("x2");
            }

            Hash = hash;
        }

        internal void Generate() => Generate(FullPath);

        protected abstract void Generate(string path);
    }
}
