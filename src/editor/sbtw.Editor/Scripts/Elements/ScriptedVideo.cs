// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Scripts.Elements
{
    public class ScriptedVideo : IScriptElement
    {
        public string Path { get; }
        public IScript Owner { get; }
        public Group Group { get; }
        public Layer Layer { get; }
        public double StartTime { get; }
        public Vector2 Position { get; }

        public ScriptedVideo(IScript owner, Group group, string path, double startTime, Vector2 position)
        {
            Owner = owner;
            Group = group;
            Path = path;
            StartTime = startTime;
            Position = position;
        }
    }
}
