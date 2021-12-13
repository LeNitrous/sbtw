// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game;
using sbtw.Editor.Projects;

namespace sbtw.Editor
{
    public class EditorBase : OsuGameBase
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load()
        {
            dependencies.CacheAs(this);

            var projectManager = new ProjectManager(Host, new DummyProject());
            dependencies.CacheAs(projectManager);
            dependencies.CacheAs<Bindable<IProject>>(new NonNullableBindable<IProject>(projectManager.DefaultProject));
        }
    }
}
