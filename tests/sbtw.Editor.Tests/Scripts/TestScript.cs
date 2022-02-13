// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Tests.Scripts
{
    public class TestScript : BuiltInScript
    {
        public Action<dynamic> Action;

        protected override void Perform(dynamic context)
        {
            Action?.Invoke(context);
        }
    }
}
