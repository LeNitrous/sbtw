// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace sbtw.Editor.Configuration
{
    public class JsonBackedConfigManager : IConfigManager, IDisposable
    {
        private readonly object saveLock = new object();
        private readonly Storage storage;
        private Dictionary<string, object> store;
        private bool isDisposed;
        private bool hasLoaded;
        private int lastSave;

        public virtual string Filename { get; }

        public JsonBackedConfigManager(Storage storage)
        {
            this.storage = storage;
        }

        protected virtual JsonSerializerSettings GetSerializerSettings() => new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public TValue Get<TValue>(string key)
        {
            if (!hasLoaded)
                throw new InvalidOperationException($"{GetType()} has not yet loaded.");

            if (store.TryGetValue(key, out var value))
                return (TValue)value;

            return default;
        }

        public bool Set<TValue>(string key, TValue value)
        {
            if (!hasLoaded)
                throw new InvalidOperationException($"{GetType()} has not yet loaded.");

            store[key] = value;
            QueueBackgroundSave();
            return true;
        }

        public void Load()
        {
            PerformLoad();
            hasLoaded = true;
        }

        public bool Save()
        {
            if (!hasLoaded)
                return false;

            lock (saveLock)
            {
                Interlocked.Increment(ref lastSave);
                return PerformSave();
            }
        }

        protected void PerformLoad()
        {
            using var stream = storage.GetStream(Filename);

            if (stream == null)
            {
                store = new Dictionary<string, object>();
                return;
            }

            using var reader = new StreamReader(stream);
            store = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.ReadToEnd(), GetSerializerSettings());
        }

        protected bool PerformSave()
        {
            try
            {
                using var stream = storage.GetStream(Filename, FileAccess.Write, FileMode.Create);
                using var writer = new StreamWriter(stream);
                writer.Write(JsonConvert.SerializeObject(store, GetSerializerSettings()));
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void QueueBackgroundSave()
        {
            int current = Interlocked.Increment(ref lastSave);

            Task.Delay(100).ContinueWith(task =>
            {
                if (current == lastSave)
                    Save();
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            Save();
            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
