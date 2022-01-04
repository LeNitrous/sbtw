// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace sbtw.Editor.Scripts.Graphics
{
    public abstract class Asset
    {
        protected abstract string Extension { get; }

        protected Script Script { get; private set; }

        protected abstract string CreateIdentifier();

        internal string Generate(Script script, string path)
        {
            Script = script;

            using var md5 = MD5.Create();

            string hash = string.Empty;
            foreach (byte part in md5.ComputeHash(Encoding.UTF8.GetBytes(CreateIdentifier())))
                hash += part.ToString("x2");

            string filePath = Path.Combine(path, Path.ChangeExtension(hash, Extension));
            string fullPath = Path.Combine(Script.Storage.GetStorageForDirectory("Beatmap").GetFullPath("."), filePath);

            if (!File.Exists(fullPath))
                Generate(fullPath);

            return filePath.Replace("\\", "/");
        }

        protected abstract void Generate(string path);
    }
}
