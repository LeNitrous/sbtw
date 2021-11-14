// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using sbtw.Game.Projects;

namespace sbtw.Game
{
    public class NetProcessListener : Component
    {
        public bool Ready => state.Value == NetProcessStatus.Exited;

        public IBindable<NetProcessStatus> State => state;

        private readonly Bindable<NetProcessStatus> state = new Bindable<NetProcessStatus>();

        public NetProcessListener()
        {
            ProjectHelper.OnDotNetExit += () => state.Value = NetProcessStatus.Exited;
            ProjectHelper.OnDotNetStart += args =>
            {
                switch (args.Split(' ').First())
                {
                    case "build":
                        state.Value = NetProcessStatus.Building;
                        break;

                    case "clean":
                        state.Value = NetProcessStatus.Cleaning;
                        break;

                    case "restore":
                        state.Value = NetProcessStatus.Restoring;
                        break;
                }
            };
        }
    }

    public enum NetProcessStatus
    {
        /// <summary>
        /// Denotes that the process has exited.
        /// </summary>
        Exited,

        /// <summary>
        /// Denotes that the process started from a build command.
        /// </summary>
        Building,

        /// <summary>
        /// Denotes that the process started form a clean command.
        /// </summary>
        Cleaning,

        /// <summary>
        /// Denotes that the process started from a restore command.
        /// </summary>
        Restoring,
    }
}
