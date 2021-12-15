// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Game.Online.API.Requests.Responses;

namespace sbtw.Editor.Overlays.Output
{
    public static class OutputUser
    {
        public static readonly APIUser USER_VERBOSE = new APIUser
        {
            Id = 0,
            Username = "verbose",
            Colour = "425e8a",
        };

        public static readonly APIUser USER_ERROR = new APIUser
        {
            Id = 1,
            Username = "error",
            Colour = "d43737",
        };

        public static readonly APIUser USER_DEBUG = new APIUser
        {
            Id = 2,
            Username = "debug",
            Colour = "949494",
        };
    }
}
