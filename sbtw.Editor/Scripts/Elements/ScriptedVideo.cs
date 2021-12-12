// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Storyboards;

namespace sbtw.Editor.Scripts.Elements
{
    public class ScriptedVideo : IScriptedElement
    {
        public string Path { get; }

        public Script Owner { get; }

        public Layer Layer => Layer.Video;

        public double StartTime { get; }

        public ScriptedVideo(Script owner, string path, int offset)
        {
            Owner = owner;
            Path = path;
            StartTime = offset;
        }
    }
}
