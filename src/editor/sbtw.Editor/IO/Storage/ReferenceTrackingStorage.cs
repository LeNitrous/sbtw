// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.IO;

namespace sbtw.Editor.IO.Storage
{
    public class ReferenceTrackingStorage : osu.Framework.Platform.Storage
    {
        public IReadOnlySet<string> References => references;

        private readonly osu.Framework.Platform.Storage storage;
        private readonly HashSet<string> references = new HashSet<string>(EqualityComparer<string>.Default);

        public ReferenceTrackingStorage(osu.Framework.Platform.Storage storage, string subfolder = null)
            : base(storage.GetFullPath("."), subfolder)
        {
            this.storage = storage;
        }

        public void Reset() => references.Clear();

        public override void Delete(string path)
        {
            references.Remove(path);
            storage.Delete(path);
        }

        public override void DeleteDirectory(string path)
            => storage.DeleteDirectory(path);

        public override bool Exists(string path)
            => storage.Exists(path);

        public override bool ExistsDirectory(string path)
            => storage.ExistsDirectory(path);

        public override IEnumerable<string> GetDirectories(string path)
            => storage.GetDirectories(path);

        public override IEnumerable<string> GetFiles(string path, string pattern = "*")
            => storage.GetFiles(path, pattern);

        public override string GetFullPath(string path, bool createIfNotExisting = false)
            => storage.GetFullPath(path, createIfNotExisting);

        public override Stream GetStream(string path, FileAccess access = FileAccess.Read, FileMode mode = FileMode.OpenOrCreate)
        {
            references.Add(path);
            return storage.GetStream(path, access, mode);
        }

        public override void OpenFileExternally(string filename)
            => storage.OpenFileExternally(filename);

        public override void PresentFileExternally(string filename)
            => storage.PresentFileExternally(filename);
    }
}
