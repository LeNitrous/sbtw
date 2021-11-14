// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Internal;
using osu.Framework.Audio.Track;
using osu.Game.Beatmaps;

namespace sbtw.Common.Scripting
{
    /// <summary>
    /// A scriptable storyboard effect.
    /// </summary>
    public abstract class StoryboardScript
    {
        /// <summary>
        /// The active beatmap.
        /// </summary>
        public IBeatmap Beatmap { get; init; }

        /// <summary>
        /// The waveform of the current beatmap's track.
        /// </summary>
        public Waveform Waveform { get; init; }

        private readonly List<StoryboardScriptElementGroup> groups = new List<StoryboardScriptElementGroup>();

        internal IEnumerable<StoryboardScriptElementGroup> Groups => groups;

        /// <summary>
        /// The entry point for this script. This is where all storyboard elements will be added.
        /// </summary>
        public abstract void Generate();

        internal Task GenerateAsync(CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            Generate();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets an element group by name.
        /// </summary>
        public StoryboardScriptElementGroup GetGroup(string name)
        {
            var group = Groups.FirstOrDefault(g => g.Name == name);

            if (group != null)
                return group;

            group = new StoryboardScriptElementGroup(this, name);
            groups.Add(group);

            return group;
        }

        /// <summary>
        /// Creates a video.
        /// </summary>
        public void SetVideo(string path, int offset)
        {
            if (Groups.Any(g => g.Name == "Video"))
                throw new InvalidOperationException("Cannot create another video in the same difficulty.");

            GetGroup("Video").CreateVideo(path, offset);
        }

        internal IEnumerable<ConfigurableMember> GetConfigurableMembers()
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            foreach (var member in GetType().GetFields(flags).Cast<MemberInfo>().Concat(GetType().GetProperties(flags)))
            {
                var attrib = Attribute.GetCustomAttribute(member, typeof(ConfigurableAttribute));
                if (attrib is not ConfigurableAttribute configurable)
                    continue;

                Type type = null;
                switch (member)
                {
                    case FieldInfo field:
                        type = field.FieldType;
                        break;

                    case PropertyInfo property:
                        type = property.PropertyType;
                        break;
                };

                if (type == null)
                    continue;

                yield return new ConfigurableMember
                {
                    Type = type,
                    Name = configurable.DisplayName ?? member.Name,
                    Order = configurable.Order,
                    Default = type.GetMemberValue(this)
                };
            }
        }
    }
}
