// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Editor.Scripts
{
    public struct ScriptVariableInfo : IEquatable<ScriptVariableInfo>
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public ScriptVariableInfo(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public bool Equals(ScriptVariableInfo other)
            => ((Name ?? string.Empty) == (other.Name ?? string.Empty)) &&
                (((Value == null) && (other.Value == null)) || Value.Equals(other.Value));

        public override bool Equals(object obj)
        {
            if (obj is not ScriptVariableInfo variable)
                return false;

            return Equals(variable);
        }

        public override int GetHashCode()
            => HashCode.Combine(Name, Value);

        public static bool operator ==(ScriptVariableInfo left, ScriptVariableInfo right)
            => left.Equals(right);

        public static bool operator !=(ScriptVariableInfo left, ScriptVariableInfo right)
            => !left.Equals(right);
    }
}
