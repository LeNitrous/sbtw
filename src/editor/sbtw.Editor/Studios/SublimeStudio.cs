// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Studios
{
    public class SublimeStudio : Studio
    {
        public override string Name => @"subl";
        public override string FriendlyName => @"Sublime";
        public override void Open(string path, int line = 0, int column = 0)
            => Run($@"-n ""{path}:{line}:{column}""");
    }
}
