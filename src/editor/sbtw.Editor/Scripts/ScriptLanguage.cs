// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    public abstract class ScriptLanguage : IScriptLanguage
    {
        public bool IsDisposed { get; private set; }

        public IEnumerable<ScriptExecutionResult> Execute<T>(T globals = null)
            where T : class
        {
            return ExecuteAsync(globals).Result;
        }

        public async Task<IEnumerable<ScriptExecutionResult>> ExecuteAsync<T>(T globals = null, CancellationToken token = default)
            where T : class
        {
            var scripts = await GetScriptsAsync(token);
            var results = new List<ScriptExecutionResult>();

            foreach (var script in scripts)
            {
                token.ThrowIfCancellationRequested();

                if (globals != null)
                {
                    foreach (var del in ScriptGlobalsHelper<T>.GetMethods(globals))
                        script.RegisterFunction(del);

                    foreach ((string name, object value) in ScriptGlobalsHelper<T>.GetValues(globals))
                        script.RegisterVariable(name, value);

                    foreach (var type in ScriptGlobalsHelper<T>.GetTypes(globals))
                        script.RegisterType(type);
                }

                var result = new ScriptExecutionResult { Script = script };

                try
                {
                    await script.ExecuteAsync(token);
                }
                catch (Exception exception)
                {
                    result.Exception = new ScriptExecutionException(GetExceptionMessage(exception), exception);
                }

                results.Add(result);
            }

            return results;
        }

        protected abstract Task<IEnumerable<IScript>> GetScriptsAsync(CancellationToken token = default);

        protected virtual string GetExceptionMessage(Exception exception)
            => exception.Message;

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
