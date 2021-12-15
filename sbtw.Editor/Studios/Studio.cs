// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Editor.Studios
{
    public class Studio : IEquatable<Studio>
    {
        public string Name { get; set; }

        public string FriendlyName { get; set; }

        public bool Equals(Studio other)
            => other.Name == Name && other.FriendlyName == FriendlyName;

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not Studio other)
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
            => HashCode.Combine(Name, FriendlyName);
    }
}
