// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlobExpressions;
using osu.Framework.Platform;

namespace sbtw.Editor.Extensions
{
    public static class StorageExtensions
    {
        public static IEnumerable<string> GetDirectories(this Storage storage, string path, string pattern = "*", SearchOption option = SearchOption.TopDirectoryOnly)
        {
            switch (option)
            {
                case SearchOption.TopDirectoryOnly:
                    return storage.GetFiles(path, pattern);

                case SearchOption.AllDirectories:
                    return getEntries(storage, path, pattern).Where(p => !Path.HasExtension(p) && Glob.IsMatch(p, pattern));

                default:
                    throw new ArgumentOutOfRangeException(nameof(option));
            }
        }

        public static IEnumerable<string> GetFiles(this Storage storage, string path, string pattern = "*", SearchOption option = SearchOption.TopDirectoryOnly)
        {
            switch (option)
            {
                case SearchOption.TopDirectoryOnly:
                    return storage.GetFiles(path, pattern);

                case SearchOption.AllDirectories:
                    return getEntries(storage, path, pattern).Where(p => Path.HasExtension(p) && Glob.IsMatch(p, pattern));

                default:
                    throw new ArgumentOutOfRangeException(nameof(option));
            }
        }

        private static IEnumerable<string> getEntries(Storage storage, string path, string pattern)
        {
            var entries = new List<string>();
            var files = storage.GetFiles(path);
            var directories = storage.GetDirectories(path);

            foreach (string file in files)
            {
                entries.Add(file);
            }

            foreach (string dir in directories)
            {
                entries.AddRange(getEntries(storage, Path.Combine(path, dir), pattern));
            }

            return entries;
        }
    }
}
