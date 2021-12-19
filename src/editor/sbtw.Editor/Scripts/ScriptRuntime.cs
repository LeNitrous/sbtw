// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts
{
    public abstract class ScriptRuntime : IDisposable
    {
        protected bool IsDisposed { get; private set; }

        public abstract Task<IEnumerable<Script>> PrepareAsync(Storage storage, CancellationToken token = default);

        public IEnumerable<Script> Prepare(Storage storage) => PrepareAsync(storage).Result;

        public virtual void GenerateResources(Storage storage)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
