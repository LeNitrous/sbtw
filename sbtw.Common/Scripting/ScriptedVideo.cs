// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Common.Scripting
{
    public class ScriptedVideo : ScriptedElement, IScriptedElementHasStartTime
    {
        internal string Path { get; private set; }

        internal int Offset { get; private set; }

        public ScriptedVideo(Script owner, string path, int offset)
            : base(owner, Layer.Background)
        {
            Path = path;
            Offset = offset;
        }

        double IScriptedElementHasStartTime.StartTime => Offset;

        internal override string Encode() => $"Video,{Offset},\"{Path}\"";
    }
}
