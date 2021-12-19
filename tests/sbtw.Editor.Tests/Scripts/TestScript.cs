// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestScript : Script
    {
        public override string Name { get; }

        private readonly Action<Script> performAction;

        public TestScript(string name, Action<Script> performAction)
        {
            Name = name;
            this.performAction = performAction;
        }

        public TestScript(string name)
            : this(name, null)
        {
        }

        protected override void Perform() => performAction?.Invoke(this);
    }
}
