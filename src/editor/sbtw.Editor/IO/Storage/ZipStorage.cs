// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using GlobExpressions;

namespace sbtw.Editor.IO.Storage
{
    public class ZipStorage : osu.Framework.Platform.Storage, IDisposable
    {
        public bool IsDisposed { get; set; }
        private readonly ZipArchive archive;

        public ZipStorage(string path, ZipArchiveMode mode = ZipArchiveMode.Read)
            : base(string.Empty)
        {
            archive = ZipFile.Open(path, mode);
        }

        public ZipStorage(Stream stream, ZipArchiveMode mode = ZipArchiveMode.Read)
            : base(string.Empty)
        {
            archive = new ZipArchive(stream, mode);
        }

        public override void Delete(string path)
            => archive.GetEntry(path).Delete();

        public override void DeleteDirectory(string path)
        {
            throw new NotSupportedException();
        }

        public override bool Exists(string path)
            => archive.GetEntry(path) != null;

        public override bool ExistsDirectory(string path)
            => false;

        public override IEnumerable<string> GetDirectories(string path)
            => Enumerable.Empty<string>();

        public override IEnumerable<string> GetFiles(string path, string pattern = "*")
            => archive.Entries.Select(entry => entry.FullName).Where(s => Glob.IsMatch(s, pattern));

        public override string GetFullPath(string path, bool createIfNotExisting = false)
        {
            throw new NotSupportedException();
        }

        public override Stream GetStream(string path, FileAccess access = FileAccess.Read, FileMode mode = FileMode.OpenOrCreate)
        {
            var memory = new MemoryStream();

            using (var stream = archive.GetEntry(path).Open())
                stream.CopyTo(memory);

            memory.Position = 0;
            return memory;
        }

        public override void OpenFileExternally(string filename)
        {
            throw new NotSupportedException();
        }

        public override void PresentFileExternally(string filename)
        {
            throw new NotSupportedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            archive.Dispose();
            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
