// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using NUnit.Framework;
using sbtw.Editor.Generators;
using sbtw.Editor.Projects;
using sbtw.Editor.Tests.Projects;
using sbtw.Editor.Tests.Scripts;

namespace sbtw.Editor.Tests.Generators
{
    public abstract class GeneratorTestBase<TGenerator, TGenerated, TElement>
        where TGenerator : Generator<TGenerated, TElement>
    {
        protected TGenerator Generator { get; private set; }
        private TestProject project;

        [SetUp]
        public void SetUp()
        {
            project = new TestProject();
            Generator = CreateGenerator(project);
        }

        [TearDown]
        public void TearDown()
        {
            Generator = null;
            project = null;
        }

        protected abstract TGenerator CreateGenerator(IProject project);

        protected TGenerated Generate(Action<dynamic> action)
        {
            (project.Language as TestScriptLanguage).Scripts = new[] { new TestScript { Action = action } };
            return Generator.Generate(null, null, true).Result;
        }
    }
}
