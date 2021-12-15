// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestScript : Script
    {
        private readonly Action<Script> performAction;

        public TestScript(Action<Script> performAction)
        {
            this.performAction = performAction;
        }

        protected override void Perform() => performAction?.Invoke(this);
    }
}
