// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using System.Threading.Tasks;
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
        protected ScriptEnvironmentStore Environments { get; private set; }
        protected StudioManager StudioManager { get; private set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            dependencies.CacheAs(this);

            dependencies.CacheAs(LocalEditorConfig);
            dependencies.CacheAs(StudioManager = CreateStudioManager());
            dependencies.CacheAs(Environments = new ScriptEnvironmentStore());

            var projectManager = new ProjectManager(Host);
            dependencies.CacheAs(projectManager);
            dependencies.CacheAs<Bindable<IProject>>(new NonNullableBindable<IProject>(new DummyProject()));
        }
        protected abstract StudioManager CreateStudioManager();

        public abstract Task<IEnumerable<string>> RequestMultipleFileAsync(string title = @"Open Files", string suggestedPath = null, IEnumerable<string> extensions = null);
        public abstract Task<string> RequestSingleFileAsync(string title = @"Open File", string suggestedPath = null, IEnumerable<string> extensions = null);
        public abstract Task<string> RequestSaveFileAsync(string title = @"Save File", string suggestedName = @"file", string suggestedPath = null, IEnumerable<string> extensions = null);
        public abstract Task<string> RequestPathAsync(string title = @"Open Folder", string suggestedPath = null);

        public IEnumerable<string> RequestMultipleFile(string title = @"Open Files", string suggestedPath = null, IEnumerable<string> extensions = null)
            => RequestMultipleFileAsync(title, suggestedPath, extensions).Result;

        public string RequestSingleFile(string title = @"Open File", string suggestedPath = null, IEnumerable<string> extensions = null)
            => RequestSingleFileAsync(title, suggestedPath, extensions).Result;

        public string RequestSaveFile(string title = @"Save File", string suggestedName = @"file", string suggestedPath = null, IEnumerable<string> extensions = null)
            => RequestSaveFileAsync(title, suggestedName, suggestedPath, extensions).Result;

        public string RequestPath(string title = @"Open Folder", string suggestedPath = null)
            => RequestPathAsync(title, suggestedPath).Result;

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
            Environments?.Dispose();
            LocalEditorConfig?.Dispose();
        }
    }
}