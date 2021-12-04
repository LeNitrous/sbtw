// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Screens;
using sbtw.Game.Projects;

namespace sbtw.Game.Screens
{
    public abstract class SBTWScreen : OsuScreen
    {
        public Bindable<IProject> Project { get; private set; }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            Project = parent?.Get<Bindable<IProject>>()?.GetBoundCopy();
            return base.CreateChildDependencies(parent);
        }
    }
}
