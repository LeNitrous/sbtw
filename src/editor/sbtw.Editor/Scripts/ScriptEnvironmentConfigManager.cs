// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Game.Database;

namespace sbtw.Editor.Scripts
{
    public abstract class ScriptEnvironmentConfigManager<TLookup> : ConfigManager<TLookup>, IScriptEnvironmentConfigManager
        where TLookup : struct, Enum
    {
        private readonly string environmentName;
        private readonly RealmContextFactory realm;
        private List<ScriptEnvironmentSetting> settings = new List<ScriptEnvironmentSetting>();

        public ScriptEnvironmentConfigManager(ScriptEnvironment environment, RealmContextFactory context)
        {
            environmentName = environment.Name;
            realm = context;

            Load();
            InitialiseDefaults();
        }

        protected override void PerformLoad()
        {
            settings = realm.Context.All<ScriptEnvironmentSetting>().Where(b => b.EnvironmentName == environmentName).ToList();
        }

        private readonly HashSet<TLookup> pendingWrites = new HashSet<TLookup>();

        protected override bool PerformSave()
        {
            TLookup[] changed;

            lock (pendingWrites)
            {
                changed = pendingWrites.ToArray();
                pendingWrites.Clear();
            }

            using var context = realm.CreateContext();
            context.Write(r =>
            {
                foreach (var c in changed)
                {
                    var setting = r.All<ScriptEnvironmentSetting>().First(s => s.EnvironmentName == environmentName && s.Key == c.ToString());
                    setting.Value = ConfigStore[c].ToString();
                }
            });

            return true;
        }

        protected override void AddBindable<TBindable>(TLookup lookup, Bindable<TBindable> bindable)
        {
            base.AddBindable(lookup, bindable);

            var setting = settings.Find(s => s.Key == lookup.ToString());

            if (setting != null)
            {
                bindable.Parse(setting.Value);
            }
            else
            {
                setting = new ScriptEnvironmentSetting
                {
                    Key = lookup.ToString(),
                    Value = bindable.Value.ToString(),
                    EnvironmentName = environmentName,
                };

                realm.Context.Write(() => realm.Context.Add(setting));
                settings.Add(setting);
            }

            bindable.ValueChanged += _ =>
            {
                lock (pendingWrites)
                    pendingWrites.Add(lookup);
            };
        }
    }
}
