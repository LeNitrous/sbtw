// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Scripts
{
    public class ScriptedSample : IScriptedElement
    {
        public string Path { get; }

        public Script Owner { get; }

        public Layer Layer { get; }

        public double StartTime { get; }

        public int Volume { get; }

        public ScriptedSample(Script owner, Layer layer, string path, double startTime, int volume)
        {
            Owner = owner;
            Layer = layer;
            Path = path;
            StartTime = startTime;
            Volume = volume;
        }
    }
}
