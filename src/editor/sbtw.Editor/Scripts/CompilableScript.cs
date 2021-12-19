// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts
{
    public abstract class CompilableScript : Script, IDisposable
    {
        public override string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

        protected bool IsDisposed { get; private set; }

        public CompilableScript(Storage storage, string path)
        {
            Path = path;
            Storage = storage;
        }

        public abstract void Compile();

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
                IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
