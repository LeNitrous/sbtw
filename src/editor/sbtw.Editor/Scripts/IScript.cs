// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    public interface IScript : IDisposable
    {
        bool Faulted { get; }
        Exception Exception { get; }
        ICanProvideAssets AssetProvider { set; }
        ICanProvideGroups GroupProvider { set; }
        ICanProvideFiles FileProvider { set; }
        ICanProvideLogger Logger { set; }
        ScriptResources Resources { set; }

        void Execute();
        Task ExecuteAsync(CancellationToken token = default);
    }
}
