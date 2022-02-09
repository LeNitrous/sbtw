// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Platform;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Assets
{
    /// <summary>
    /// Represents a generatable asset.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Asset : IEquatable<Asset>
    {
        /// <summary>
        /// The relative path to the given asset.
        /// </summary>
        [JsonProperty]
        internal string Path { get; set; }

        /// <summary>
        /// The number of times this asset was referenced.
        /// </summary>
        internal int ReferenceCount { get; set; }

        /// <summary>
        /// The script that generated this asset.
        /// </summary>
        internal IScript Owner { get; set; }

        /// <summary>
        /// The project this asset is owned by.
        /// </summary>
        internal IProject Project { get; private set; }

        /// <summary>
        /// The storage where this asset is being generated on.
        /// </summary>
        protected Storage Storage { get; private set; }

        /// <summary>
        /// Generates this asset.
        /// </summary>
        /// <param name="storage">The target storage to generate to.</param>
        internal void Generate(IProject project)
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException(@"Asset is not yet ready for generation.");

            if (project is not ICanProvideFiles fileProvider)
                throw new InvalidOperationException(@"Project cannot provide files");

            Project = project;

            using var stream = fileProvider.BeatmapFiles.GetStream(Path, FileAccess.Write, FileMode.OpenOrCreate);
            stream.Position = 0;
            stream.Write(Generate());
        }

        /// <summary>
        /// Generates this asset with a given path.
        /// </summary>
        /// <returns>The generated asset as a read-only span of bytes.</returns>
        protected abstract ReadOnlySpan<byte> Generate();

        /// <summary>
        /// Check whether the asset is equal to the other asset.
        /// </summary>
        /// <remarks>
        /// This must be overridden if the asset provides other properties.
        /// As by default it only checks for type equality only.
        /// </remarks>
        /// <param name="other">The other asset to test against.</param>
        /// <returns>True if the assets match. Otherwise false.</returns>
        public virtual bool Equals(Asset other)
            => other.GetType() == GetType();

        public override bool Equals(object obj)
            => Equals(obj as Asset);

        public override int GetHashCode()
            => HashCode.Combine(Path);
    }
}
