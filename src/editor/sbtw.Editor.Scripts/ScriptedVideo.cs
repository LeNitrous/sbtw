// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Scripts
{
    public class ScriptedVideo : IScriptedElement
    {
        public string Path { get; }

        public IScript Owner { get; }

        public Layer Layer { get; }

        public double StartTime { get; }

        public ScriptedVideo(IScript owner, string path, int offset)
        {
            Owner = owner;
            Path = path;
            StartTime = offset;
        }
    }
}
