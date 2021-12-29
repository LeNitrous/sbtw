// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using osu.Framework.IO.Stores;

namespace sbtw.Editor.Languages.Typescript.Tests
{
    internal static class TypescriptTestResources
    {
        private static readonly IResourceStore<byte[]> resources = new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(TypescriptTestResources).Assembly), "Resources");
        public static Stream GetStream(string path) => resources.GetStream(path);
    }
}
