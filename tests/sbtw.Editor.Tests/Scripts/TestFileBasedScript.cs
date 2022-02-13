// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestFileBasedScript : FileBasedScript
    {
        public readonly List<Delegate> Delegates = new List<Delegate>();
        public readonly List<Type> Types = new List<Type>();
        public readonly Dictionary<string, object> Members = new Dictionary<string, object>();
        public int CompileCount { get; private set; }
        public string Compiled { get; private set; }

        public TestFileBasedScript(string path)
            : base(path)
        {
        }

        public override async Task CompileAsync(CancellationToken token = default)
        {
            Compiled = await File.ReadAllTextAsync(Path, token);
            CompileCount++;
        }

        public override Task ExecuteAsync(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public override void RegisterFunction(Delegate del)
            => Delegates.Add(del);

        public override void RegisterVariable(string name, object value)
            => Members[name] = value;

        public override void RegisterType(Type type)
            => Types.Add(type);
    }
}
