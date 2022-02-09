// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using NUnit.Framework;
using sbtw.Editor.Generators;
using sbtw.Editor.Scripts;
using sbtw.Editor.Tests.Projects;
using sbtw.Editor.Tests.Scripts;

namespace sbtw.Editor.Tests.Generators
{
    public abstract class GeneratorTestBase<TGenerator, TGenerated, TElement>
        where TGenerator : Generator<TGenerated, TElement>
    {
        protected TGenerator Generator { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Generator = CreateGenerator();
        }

        [TearDown]
        public void TearDown()
        {
            Generator = null;
        }

        protected abstract TGenerator CreateGenerator();

        protected TGenerated Generate(Action<Script> action)
        {
            var provider = new TestProject();
            (provider.Language as TestScriptLanguage).Scripts = new[] { new TestScript { Action = action } };
            return Generator.Generate(provider, target: null, includeHidden: true);
        }
    }
}
