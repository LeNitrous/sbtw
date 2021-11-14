// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Common.Scripting
{
    internal struct ConfigurableMember
    {
        /// <summary>
        /// The configurable member's type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The configurable member's display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The configurable member's display order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The configurable member's default value.
        /// </summary>
        public object Default { get; set; }
    }
}
