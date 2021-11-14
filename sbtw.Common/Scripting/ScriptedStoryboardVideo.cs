// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public class ScriptedStoryboardVideo : IScriptedElementHasStartTime
    {
        public StoryboardScript Owner { get; private set; }

        public StoryboardLayerName Layer { get; private set; }

        public string Path { get; private set; }

        public int Offset { get; private set; }

        public ScriptedStoryboardVideo(StoryboardScript owner, string path, int offset)
        {
            Path = path;
            Owner = owner;
            Offset = offset;
        }

        double IScriptedElementHasStartTime.StartTime => Offset;
    }
}
