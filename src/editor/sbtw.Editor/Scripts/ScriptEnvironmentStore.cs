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
    public class ScriptEnvironmentStore : IDisposable
    {
        private readonly List<ScriptEnvironment> environments = new List<ScriptEnvironment>();
        private bool isDisposed;

        public void Register(ScriptEnvironment environment)
        {
            if (environments.Any(e => e.GetType() == environment.GetType()))
                throw new InvalidOperationException(@"An environment of this type already exists in this store");

            environments.Add(environment);
        }

        public ScriptEnvironment GetEnvironment(string name)
            => environments.FirstOrDefault(e => e.Name == name);

        public async Task<IEnumerable<Script>> PrepareAsync(Storage storage, CancellationToken token = default)
        {
            var scripts = new List<Script>();

            foreach (var environment in environments)
                scripts.AddRange(await environment.Runtime.PrepareAsync(storage, token));

            return scripts;
        }

        public IEnumerable<Script> Prepare(Storage storage) => PrepareAsync(storage).Result;

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed && !disposing)
                return;

            foreach (var environment in environments)
                environment.Dispose();

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
