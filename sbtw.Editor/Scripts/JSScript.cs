// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts
{
    public class JSScript : Script, IDisposable
    {
        public readonly string Path;

        private readonly V8ScriptEngine engine;
        private readonly Storage storage;
        private V8Script compiled;
        private DateTimeOffset lastCompileTime;
        private bool isDisposed;

        public JSScript(V8ScriptEngine engine, Storage storage, string path)
        {
            Path = path;
            this.storage = storage;
            this.engine = engine;
            this.engine.AddHostObject("SetVideo", new Action<string, int>(SetVideo));
            this.engine.AddHostObject("OpenFile", new Func<string, ITypedArray<byte>>(OpenFile));
            this.engine.AddHostObject("GetGroup", new Func<string, ScriptElementGroup>(GetGroup));
        }

        public ITypedArray<byte> OpenFile(string path)
        {
            dynamic createUint8Array = engine.Evaluate("new Function('data', 'return new Uint8Array(data)')");
            using var stream = storage.GetStream(path, FileAccess.Read, FileMode.Open);
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            return createUint8Array(memory.ToArray());
        }

        public void Compile()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(JSScript));

            if (lastCompileTime != File.GetLastWriteTime(Path))
            {
                compiled?.Dispose();
                compiled = engine.CompileDocument(Path);
                lastCompileTime = File.GetLastWriteTime(Path);
            }
        }

        protected override void Perform()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(JSScript));

            if (compiled == null)
                return;

            engine.Execute(compiled);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    engine.Dispose();
                    compiled?.Dispose();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
