// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

namespace sbtw.Editor.Platform.Windows
{
    internal enum HResult : uint
    {
        OK = 0x0000,
        False = 0x0001,
        InvalidArgument = 0x80070057,
        OutOfMemory = 0x8007000E
    }
}
