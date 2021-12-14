// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.IO;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace sbtw.Editor.Projects
{
    public class Project : IProject
    {
        public string Name { get; }
        public string Path { get; }

        [JsonIgnore]
        public readonly Storage Storage;

        public Project(Storage storage, string name)
        {
            Name = name;
            Path = storage.GetFullPath(".");
            Storage = storage;
        }

        public bool Save()
        {
            try
            {
                using var stream = Storage.GetStream(System.IO.Path.ChangeExtension(Name, ".sbtw.json"), FileAccess.Write);
                using var writer = new StreamWriter(stream);
                stream.SetLength(0);
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to save project");
                return false;
            }
        }
    }
}
