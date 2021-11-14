// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public class ScriptedStoryboardSample : IScriptedElementHasStartTime
    {
        public StoryboardScript Owner { get; private set; }

        public StoryboardLayerName Layer { get; private set; }

        /// <summary>
        /// The path to the audio file for this sample.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The time when this sample is triggered.
        /// </summary>
        public double Time { get; private set; }

        /// <summary>
        /// The loudness of this sample.
        /// </summary>
        public int Volume { get; private set; }

        public ScriptedStoryboardSample(StoryboardScript owner, StoryboardLayerName layer, string path, double time, int volume)
        {
            Path = path;
            Time = time;
            Owner = owner;
            Layer = layer;
            Volume = volume;
        }

        double IScriptedElementHasStartTime.StartTime => Time;
    }
}
