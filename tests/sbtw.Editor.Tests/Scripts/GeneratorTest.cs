// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public abstract class GeneratorTest<TRunner, TResult, TGenerated>
        where TRunner : ScriptRunner<TResult, TGenerated>
        where TResult : new()
    {
        protected TRunner Encoder { get; private set; }

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

        protected abstract TRunner CreateRunner();

        protected ScriptRunnerGenerationResult<TResult, TGenerated> Generate(Action<Script> perform = null, IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> variables = null, IEnumerable<string> ordering = null)
            => Generate(Guid.NewGuid().ToString(), perform, variables, ordering);

        protected ScriptRunnerGenerationResult<TResult, TGenerated> Generate(string name, Action<Script> perform = null, IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> variables = null, IEnumerable<string> ordering = null)
            => Generate(new Dictionary<string, Action<Script>> { { name, perform } }, variables, ordering);

        protected ScriptRunnerGenerationResult<TResult, TGenerated> Generate(IReadOnlyDictionary<string, Action<Script>> scripts, IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> variables = null, IEnumerable<string> ordering = null)
            => Encoder.Generate(new ScriptRunnerGenerationConfiguration { Scripts = scripts.Select(p => new TestScript(p.Key, p.Value)), Variables = variables, Ordering = ordering });
    }
}
