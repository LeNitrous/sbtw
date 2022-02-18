// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Studios
{
    public class NoStudio : Studio
    {
        public override string Name => @"none";
        public override string FriendlyName => @"None";

        public override void Open(string path, int line = 0, int column = 0)
        {
        }
    }
}
