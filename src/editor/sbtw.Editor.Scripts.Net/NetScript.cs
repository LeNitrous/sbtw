// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Platform;

namespace sbtw.Editor.Scripts.Net
{
    public class NetScript : CompilableScript
    {
        public override string Name => script.Name;

        private readonly Script script;

        public NetScript(Script script, Storage storage, string path)
            : base(storage, path)
        {
            this.script = script;
        }

        public override void Compile()
        {
        }

        protected override void Perform()
        {
        }
    }
}
