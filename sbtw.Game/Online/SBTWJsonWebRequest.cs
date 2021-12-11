// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.IO.Network;

namespace sbtw.Game.Online
{
    public class SBTWJsonWebRequest<T> : JsonWebRequest<T>
    {
        protected override string UserAgent => "sbtw!";

        public SBTWJsonWebRequest(string url)
            : base(url, Array.Empty<object>())
        {
        }

        public SBTWJsonWebRequest()
            : base(null, Array.Empty<object>())
        {
        }
    }
}
