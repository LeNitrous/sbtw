// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// Represents scripts of other languages that require external files for execution.
    /// </summary>
    public abstract class FileBasedScript : IScript, IDisposable, INamedScript
    {
        public string Name => System.IO.Path.GetFileName(Path);

        /// <summary>
        /// The full path for this script.
        /// </summary>
        public readonly string Path;
        public bool IsDisposed { get; private set; }

        public FileBasedScript(string path)
        {
            Path = path;
        }

        public void Compile() => CompileAsync().Wait();
        public abstract Task CompileAsync(CancellationToken token = default);
        public abstract Task ExecuteAsync(CancellationToken token = default);
        public abstract void RegisterType(Type type);
        public abstract void RegisterFunction(Delegate del);
        public abstract void RegisterVariable(string name, object value);

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
