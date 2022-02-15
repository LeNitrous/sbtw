// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Platform;
using sbtw.Editor.IO.Serialization;

namespace sbtw.Editor.Scripts.Assets
{
    /// <summary>
    /// Represents a generatable asset.
    /// </summary>
    [JsonConverter(typeof(AssetConverter))]
    public abstract class Asset : IEquatable<Asset>
    {
        /// <summary>
        /// The relative path to the given asset.
        /// </summary>
        internal string Path { get; set; }

        /// <summary>
        /// The number of times this asset was referenced.
        /// </summary>
        public int ReferenceCount { get; set; }

        /// <summary>
        /// The storage where this asset is being generated on.
        /// </summary>
        protected Storage Storage { get; private set; }

        /// <summary>
        /// Generates this asset.
        /// </summary>
        /// <param name="storage">The target storage to generate to.</param>
        internal void Generate(Storage storage)
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException(@"Asset is not yet ready for generation.");

            Storage = storage;

            using var stream = Storage.GetStream(Path, FileAccess.Write, FileMode.OpenOrCreate);
            stream.Position = 0;
            stream.Write(Generate());
        }

        /// <summary>
        /// Generates this asset with a given path.
        /// </summary>
        /// <returns>The generated asset as a read-only span of bytes.</returns>
        protected abstract ReadOnlySpan<byte> Generate();

        /// <summary>
        /// Check whether the asset is equal to the other asset by its path.
        /// </summary>
        public bool Equals(Asset other)
            => other.Path.Equals(Path);

        public override bool Equals(object obj)
            => Equals(obj as Asset);

        public override int GetHashCode()
            => HashCode.Combine(Path);
    }
}
