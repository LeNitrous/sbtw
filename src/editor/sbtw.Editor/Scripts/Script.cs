// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Logging;
using sbtw.Editor.Assets;
using sbtw.Editor.Projects;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// Represents a script.
    /// </summary>
    public abstract class Script : IScript, IDisposable
    {
        /// <summary>
        /// Gets the exception raised during its last execution.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets whether this script has any exceptions raised during its last execution.
        /// </summary>
        public bool Faulted => Exception != null;

        /// <summary>
        /// Gets whether this script has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets the asset provider for this script.
        /// </summary>
        public ICanProvideAssets AssetProvider { get; set; }

        /// <summary>
        /// Gets or sets the group provider for this script.
        /// </summary>
        public ICanProvideGroups GroupProvider { get; set; }

        /// <summary>
        /// Gets or sets the file provider for this script.
        /// </summary>
        public ICanProvideFiles FileProvider { get; set; }

        /// <summary>
        /// Gets or sets this script's log provider.
        /// </summary>
        public ICanProvideLogger Logger { get; set; }

        /// <summary>
        /// Executes this script.
        /// </summary>
        public void Execute()
            => ExecuteAsync().Wait();

        /// <summary>
        /// Executes this script asynchronously.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            try
            {
                await PerformAsync();
                Exception = null;
            }
            catch (Exception e)
            {
                Exception = e;
            }
        }

        /// <summary>
        /// Executes this script.
        /// </summary>
        /// <returns>The task used to run this method.</returns>
        protected abstract Task PerformAsync();

        [ScriptVisible]
        public string GetAsset(string path, Asset asset)
        {
            if (AssetProvider == null)
                throw new NotSupportedException(@"This script does not support getting assets.");

            asset.Path = path;
            asset.Owner = this;
            AssetProvider.Assets.Add(asset);
            return path;
        }

        [ScriptVisible]
        public Group GetGroup(string name)
        {
            if (GroupProvider == null)
                throw new NotSupportedException(@"This script does not support getting groups.");

            var group = GroupProvider.Groups.FirstOrDefault(g => g.Name == name);

            if (group != null)
                return group;

            GroupProvider.Groups.Add(group = new Group(name));

            return group;
        }

        [ScriptVisible]
        public byte[] Fetch(string path)
        {
            if (FileProvider == null)
                throw new NotSupportedException(@"This script does not support storage access.");

            if (!FileProvider.Files.Exists(path))
                throw new FileNotFoundException($@"File ""{path}"" does not exist.");

            using var stream = FileProvider.Files.GetStream(path, FileAccess.Read, FileMode.OpenOrCreate);
            using var memory = new MemoryStream();
            stream.CopyTo(memory);

            return memory.ToArray();
        }

        [ScriptVisible]
        public string Fetch(string path, bool _)
            => Encoding.Default.GetString(Fetch(path));

        [ScriptVisible]
        public void Log(object message)
            => Log(message, LogLevel.Debug);

        [ScriptVisible]
        public void Error(object message)
            => Log(message, LogLevel.Error);

        protected void Log(object message, LogLevel level = LogLevel.Debug)
        {
            if (Logger == null)
                throw new NotSupportedException(@"This script does not support logging.");

            Logger.Log(message, level);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
                IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
