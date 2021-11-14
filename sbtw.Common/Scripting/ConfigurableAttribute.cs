// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Common.Scripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigurableAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public int Order { get; set; }

        public ConfigurableAttribute(string displayName = null, int order = 0)
        {
            DisplayName = displayName;
            Order = order;
        }
    }
}
