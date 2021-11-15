// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public interface IScriptedElement
    {
        /// <summary>
        /// The <see cref="Script"/> that instantiated this element.
        /// </summary>
        Script Owner { get; }

        /// <summary>
        /// The <see cref="Scripting.Layer"/> for this element.
        /// </summary>
        Layer Layer { get; }
    }
}
