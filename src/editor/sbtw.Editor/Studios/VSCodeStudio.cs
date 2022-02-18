// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Studios
{
    public class VSCodeStudio : Studio
    {
        public override string Name => @"code";
        public override string FriendlyName => @"Visual Studio Code";
        public override void Open(string path, int line = 0, int column = 0)
            => Run($@"-g ""{path}:{line}:{column}""");
    }
}
