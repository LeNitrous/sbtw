// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using NUnit.Framework;
using sbtw.Editor.Scripts;
using sbtw.Editor.Tests.Scripts;

namespace sbtw.Editor.Tests.Storyboards
{
    public abstract class ScriptRunnerTest<TGenerator, TGenerated>
        where TGenerator : ScriptRunner<TGenerated>
        where TGenerated : new()
    {
        protected TGenerator Encoder { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Encoder = CreateRunner();
        }

        [TearDown]
        public void TearDown()
        {
            Encoder = null;
        }

        protected abstract TGenerator CreateRunner();

        protected TGenerated Generate(Action<Script> perform = null) => Encoder.Generate(new[] { new TestScript(perform) });
    }
}
