// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestScript : Script
    {
        private readonly Action<Script> performAction;

        public TestScript(string name, Action<Script> performAction)
            : base(name, string.Empty)
        {
            this.performAction = performAction;
        }

        public TestScript(string name)
            : this(name, null)
        {
        }

        protected override void Perform() => performAction?.Invoke(this);

        protected override void RegisterMethod(string name, Delegate method)
        {
        }
    }
}
