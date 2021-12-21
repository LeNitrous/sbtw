// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.IO;
using NUnit.Framework;
using osu.Framework.Testing;
using sbtw.Editor.Languages.Javascript.Tests.Resources;

namespace sbtw.Editor.Languages.Javascript.Tests
{
    [SetUpFixture]
    public class SetUpTests
    {
        public static TemporaryNativeStorage Storage { get; protected set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Storage = new TemporaryNativeStorage($"{nameof(TestScripts)}");
            copy("throw");
            copy("script");
            copy("importing");
            copy("importable");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Storage?.Dispose();
        }

        private static void copy(string name)
        {
            foreach (string ext in new[] { "js", "ts" })
            {
                string filename = $"{name}.{ext}";
                using var rStream = TestResources.GetStream(filename);
                using var wStream = Storage.GetStream(filename, FileAccess.Write);
                wStream.Position = 0;
                rStream.CopyTo(wStream);
            }
        }
    }
}
