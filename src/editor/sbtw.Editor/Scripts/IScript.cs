// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    public interface IScript
    {
        void Execute() => ExecuteAsync();
        Task ExecuteAsync(CancellationToken token = default);

        void RegisterType(Type type);
        void RegisterFunction(Delegate del);
        void RegisterVariable(string name, object value);
    }
}
