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
        private ScriptManager manager;
        private TestProject project;
        private TestScriptLanguage language;

        [SetUp]
        public void SetUp()
        {
            project = new TestProject();
            manager = new ScriptManager(project.Files);
            manager.AddLanguage(language = new TestScriptLanguage());
            Generator = CreateGenerator(manager);
        }

        [TearDown]
        public void TearDown()
        {
            Generator = null;
            language = null;
            manager = null;
            project = null;
        }

        protected abstract TGenerator CreateGenerator(ScriptManager manager);

        protected TGenerated Generate(Action<dynamic> action)
        {
            language.Scripts = new[] { new TestScript { Action = action } };
            return Generator.Generate(new ScriptGlobals { GroupProvider = project }).Result;
        }
    }
}
