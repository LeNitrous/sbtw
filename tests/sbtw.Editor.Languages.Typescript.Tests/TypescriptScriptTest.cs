// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using NUnit.Framework;
using sbtw.Editor.Languages.Javascript.Scripts;
using sbtw.Editor.Languages.Javascript.Tests;
using sbtw.Editor.Scripts;
using TSCompiler = sbtw.Editor.Languages.Javascript.Scripts.Typescript;

namespace sbtw.Editor.Languages.Typescript.Tests
{
    [TestFixture]
    public class TypescriptScriptTest : JavascriptScriptTest
    {
        protected override string Extension => "ts";

        protected override Script CreateScript(string path) => new TypescriptScript(CreateEngine(), compiler, string.Empty, path);
        protected override Stream GetStream(string path) => TypescriptTestResources.GetStream(path);

        private static readonly TSCompiler compiler = new TSCompiler(CreateEngine());
    }
}
