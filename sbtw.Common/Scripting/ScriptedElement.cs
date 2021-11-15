// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public abstract class ScriptedElement : IScriptedElement
    {
        public Script Owner { get; private set; }

        public Layer Layer { get; private set; }

        protected ScriptedElement(Script owner, Layer layer)
        {
            Owner = owner;
            Layer = layer;
        }

        internal abstract string Encode();
    }
}
