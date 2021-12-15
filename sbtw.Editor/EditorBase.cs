// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Game;
using sbtw.Editor.Configuration;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Studios;

namespace sbtw.Editor
{
    public abstract class EditorBase : OsuGameBase
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        protected EditorConfigManager LocalEditorConfig { get; private set; }

        protected IScriptRuntime ScriptRuntime { get; private set; }

        protected IStudioManager StudioManager { get; private set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            dependencies.CacheAs(this);

            var projectManager = new ProjectManager(Host, new DummyProject());
            dependencies.CacheAs(projectManager);
            dependencies.CacheAs<Bindable<IProject>>(new NonNullableBindable<IProject>(projectManager.DefaultProject));

            dependencies.CacheAs(LocalEditorConfig);
            dependencies.CacheAs(StudioManager = CreateStudioManager());
            dependencies.CacheAs(ScriptRuntime = CreateScriptRuntime());
        }

        protected abstract IScriptRuntime CreateScriptRuntime();
        protected abstract IStudioManager CreateStudioManager();

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            LocalEditorConfig ??= IsDeployedBuild
                ? new EditorConfigManager(Storage)
                : new DevelopmentEditorConfigManager(Storage);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            LocalEditorConfig?.Dispose();
        }
    }
}
