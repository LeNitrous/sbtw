// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Framework.Platform;
using sbtw.Editor.Scripts.Assets;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Tests.Scripts;

namespace sbtw.Editor.Tests.Projects
{
    public class TestProject : IProject, ICanProvideGroups, ICanProvideFiles, ICanProvideAssets, ICanProvideLogger, IDisposable
    {
        public BindableInt Precision { get; } = new BindableInt();
        public GroupCollection Groups { get; } = new GroupCollection();
        public Storage Files { get; }
        public Storage BeatmapFiles { get; }
        public HashSet<Asset> Assets { get; } = new HashSet<Asset>();
        public readonly List<string> Logs = new List<string>();
        private bool isDisposed;

        public TestProject()
        {
            Files = CreateStorage();
            BeatmapFiles = Files.GetStorageForDirectory("Beatmap");
        }

        protected virtual Storage CreateStorage() => new ShamStorage();
        protected virtual IEnumerable<Type> GetScriptingTypes() => new[] { typeof(TestScriptLanguage) };

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Log(object message, LogLevel level = LogLevel.Verbose)
            => Logs.Add(message.ToString());

        private class ShamStorage : NativeStorage
        {
            public ShamStorage(string path = ".", GameHost host = null)
                : base(path, host)
            {
            }

            public override void Delete(string path)
            {
                throw new NotImplementedException();
            }

            public override void DeleteDirectory(string path)
            {
                throw new NotImplementedException();
            }

            public override bool Exists(string path)
            {
                return true;
            }

            public override bool ExistsDirectory(string path)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<string> GetDirectories(string path)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<string> GetFiles(string path, string pattern = "*")
            {
                throw new NotImplementedException();
            }

            public override string GetFullPath(string path, bool createIfNotExisting = false)
            {
                return path;
            }

            public override Stream GetStream(string path, FileAccess access = FileAccess.Read, FileMode mode = FileMode.OpenOrCreate)
            {
                return new MemoryStream();
            }

            public override void OpenFileExternally(string filename)
            {
                throw new NotImplementedException();
            }

            public override void PresentFileExternally(string filename)
            {
                throw new NotImplementedException();
            }
        }
    }
}
