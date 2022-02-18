// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Studios
{
    public class NppStudio : Studio
    {
        public override string Name => @"notepad++";
        public override string FriendlyName => @"Notepad++";
        public override void Open(string path, int line = 0, int column = 0)
            => Run($@"{path} -n {line} -c {column}");
    }
}
