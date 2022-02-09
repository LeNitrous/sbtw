// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Diagnostics;

namespace sbtw.Editor.Studios
{
    public class Studio : IEquatable<Studio>
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }

        public bool Equals(Studio other)
            => other.Name == Name && other.FriendlyName == FriendlyName;

        public override bool Equals(object obj)
            => obj is Studio studio && Equals(studio);

        public override int GetHashCode()
            => HashCode.Combine(Name, FriendlyName);

        public void Open(string path) => Process.Start(new ProcessStartInfo
        {
            FileName = Name,
            Arguments = $@"""{path}""",
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true,
        });
    }
}
