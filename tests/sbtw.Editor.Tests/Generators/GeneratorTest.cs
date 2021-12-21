// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using sbtw.Editor.Generators;
using sbtw.Editor.Scripts;
using sbtw.Editor.Tests.Scripts;

namespace sbtw.Editor.Tests.Generators
{
    public abstract class GeneratorTest<TGenerator, TResult, TGenerated>
        where TGenerator : Generator<TResult, TGenerated>
    {
        protected TGenerator Generator { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Generator = CreateRunner();
        }

        [TearDown]
        public void TearDown()
        {
            Generator = null;
        }

        protected abstract TGenerator CreateRunner();

        protected GeneratorResult<TResult, TGenerated> Generate(Action<Script> perform = null, IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> variables = null, IEnumerable<string> ordering = null)
            => Generate(Guid.NewGuid().ToString(), perform, variables, ordering);

        protected GeneratorResult<TResult, TGenerated> Generate(string name, Action<Script> perform = null, IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> variables = null, IEnumerable<string> ordering = null)
            => Generate(new Dictionary<string, Action<Script>> { { name, perform } }, variables, ordering);

        protected GeneratorResult<TResult, TGenerated> Generate(IReadOnlyDictionary<string, Action<Script>> scripts, IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> variables = null, IEnumerable<string> ordering = null)
            => Generator.Generate(new GeneratorConfig { Scripts = scripts.Select(p => new TestScript(p.Key, p.Value)), Variables = variables, Ordering = ordering });
    }

    public abstract class GeneratorTest<TGenerator, TResult> : GeneratorTest<TGenerator, TResult, TResult>
        where TGenerator : Generator<TResult>
    {
    }
}
