// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Platform;

namespace sbtw.Editor.Scripts
{
    public abstract class ScriptCompilingRuntime : ScriptRuntime
    {
        protected abstract string Extension { get; }

        private readonly List<ScriptInfo> lastScriptInfos = new List<ScriptInfo>();

        public override Task<IEnumerable<Script>> PrepareAsync(Storage storage, CancellationToken token = default)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            var scriptInfos = new List<ScriptInfo>();

            foreach (var path in storage.GetFiles(".", $"*.{Extension}"))
            {
                var scriptInfo = lastScriptInfos.FirstOrDefault(s => s.Path == path);

                if (scriptInfo != null)
                {
                    if (scriptInfo.LastCompileTime != DateTimeOffset.Now)
                    {
                        scriptInfos.Add(scriptInfo);
                        scriptInfo.LastCompileTime = DateTimeOffset.Now;
                    }
                }
                else
                {
                    scriptInfos.Add(new ScriptInfo(CreateScript(storage, path)));
                }
            }

            foreach (var scriptInfo in scriptInfos)
                scriptInfo.Script.Compile();

            clear();

            lastScriptInfos.AddRange(scriptInfos);

            var task = new TaskCompletionSource<IEnumerable<Script>>();
            task.SetResult(scriptInfos.Select(s => s.Script));

            return task.Task;
        }

        protected abstract CompilableScript CreateScript(Storage storage, string path);

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            clear();

            base.Dispose(disposing);
        }

        private void clear()
        {
            foreach (var scriptInfo in lastScriptInfos)
                scriptInfo.Script.Dispose();

            lastScriptInfos.Clear();
        }

        private class ScriptInfo
        {
            public CompilableScript Script { get; set; }
            public string Path => Script.Path;
            public DateTimeOffset LastCompileTime { get; set; }

            public ScriptInfo(CompilableScript script)
            {
                Script = script;
                LastCompileTime = DateTimeOffset.Now;
            }
        }
    }
}
