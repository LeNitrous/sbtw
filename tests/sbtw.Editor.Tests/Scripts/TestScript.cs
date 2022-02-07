// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Threading.Tasks;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestScript : Script
    {
        public Action Action;

        protected override Task PerformAsync()
        {
            Action?.Invoke();
            return Task.CompletedTask;
        }
    }
}
