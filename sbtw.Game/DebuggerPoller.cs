// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Diagnostics;
using System.Threading.Tasks;
using osu.Framework.Bindables;
using osu.Game.Online;

namespace sbtw.Game
{
    /// <summary>
    /// A component that polls for debuggers being attached.
    /// </summary>
    public class DebuggerPoller : PollingComponent
    {
        /// <summary>
        /// Gets whether a debugger has been attached.
        /// </summary>
        public IBindable<bool> Attached => attached;

        private readonly Bindable<bool> attached = new Bindable<bool>();

        public DebuggerPoller()
            : base(1000)
        {
        }

        protected override Task Poll()
        {
            attached.Value = Debugger.IsAttached;
            return base.Poll();
        }
    }
}
