// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Reflection;
using System.Runtime.Loader;

namespace sbtw.Game.Scripting
{
    public class ScriptAssemblyContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public ScriptAssemblyContext(string scriptPath)
            : base(true)
        {
            resolver = new AssemblyDependencyResolver(scriptPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string path = resolver.ResolveAssemblyToPath(assemblyName);

            if (!string.IsNullOrEmpty(path))
                return LoadFromAssemblyName(assemblyName);

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string path = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (!string.IsNullOrEmpty(path))
                return LoadUnmanagedDllFromPath(path);

            return IntPtr.Zero;
        }
    }
}
