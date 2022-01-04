// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Scripts.Elements
{
    public class ScriptedVideo : IScriptedElement
    {
        public string Path { get; }

        public Script Owner { get; }

        public string Group { get; }

        public Layer Layer { get; }

        public double StartTime { get; }

        public ScriptedVideo(Script owner, string group, string path, int offset)
        {
            Owner = owner;
            Group = group;
            Path = path;
            StartTime = offset;
        }
    }
}
