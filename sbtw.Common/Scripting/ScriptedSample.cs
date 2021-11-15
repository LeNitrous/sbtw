// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Common.Scripting
{
    public class ScriptedSample : ScriptedElement, IScriptedElementHasStartTime
    {
        internal string Path { get; private set; }

        internal double Time { get; private set; }

        internal int Volume { get; private set; }

        public ScriptedSample(Script owner, Layer layer, string path, double time, int volume)
            : base(owner, layer)
        {
            Path = path;
            Time = time;
            Volume = volume;
        }

        double IScriptedElementHasStartTime.StartTime => Time;

        internal override string Encode() => $"Sample,{Time},{Enum.GetName(Layer)},\"{Path}\",{Volume}";
    }
}
