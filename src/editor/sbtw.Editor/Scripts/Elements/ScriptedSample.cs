// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Scripts.Elements
{
    public class ScriptedSample : IScriptedElement
    {
        public string Path { get; }

        public Script Owner { get; }

        public string Group { get; }

        public Layer Layer { get; }

        public double StartTime { get; }

        public int Volume { get; }

        public ScriptedSample(Script owner, string group, Layer layer, string path, double startTime, int volume)
        {
            Owner = owner;
            Group = group;
            Layer = layer;
            Path = path;
            StartTime = startTime;
            Volume = volume;
        }
    }
}
