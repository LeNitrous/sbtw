// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using osu.Framework.IO.Stores;

namespace sbtw.Editor.Languages.Lua.Tests
{
    internal static class LuaTestResources
    {
        private static readonly IResourceStore<byte[]> resources = new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(LuaTestResources).Assembly), "Resources");
        public static Stream GetStream(string path) => resources.GetStream(path);
    }
}
