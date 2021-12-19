// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.IO.Stores;

namespace sbtw.Editor.Scripts.Javascript.Resources
{
    public static class ResourceAssembly
    {
        public static readonly IResourceStore<byte[]> Resources = new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(ResourceAssembly).Assembly), "Resources");
    }
}
