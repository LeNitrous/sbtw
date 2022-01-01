// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Scripts
{
    public class ScriptVariableInfo
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public ScriptVariableInfo(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
