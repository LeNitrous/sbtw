// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Lists;
using osu.Game.Storyboards;
using osuTK;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// Represents a collection of elements.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Group
    {
        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonProperty]
        public string Name { get; }

        /// <summary>
        /// The <see cref="GroupCollection"/> this group is provided by.
        /// </summary>
        internal GroupCollection Provider;

        /// <summary>
        /// Whether this group should be visible in the editor.
        /// </summary>
        [JsonProperty]
        internal BindableBool Visible { get; } = new BindableBool(true);

        /// <summary>
        /// Determines where this group should be exported to.
        /// </summary>
        [JsonProperty]
        internal Bindable<ExportTarget> Target { get; } = new Bindable<ExportTarget>(ExportTarget.Storyboard);

        /// <summary>
        /// Gets the elements this group has.
        /// </summary>
        public IReadOnlyList<IScriptElement> Elements => elements;

        /// <summary>
        /// Gets the start time for this group.
        /// </summary>
        public double StartTime => elements.Min(e => e.StartTime);

        /// <summary>
        /// Gets the end time for this group.
        /// </summary>
        public double EndTime => elements.Max(e => (e as IScriptElementHasDuration)?.EndTime ?? e.StartTime);

        /// <summary>
        /// Gets the duration for this group.
        /// </summary>
        public double Duration => EndTime - StartTime;

        private readonly SortedList<IScriptElement> elements = new SortedList<IScriptElement>(new ScriptedElementComparer());

        [JsonConstructor]
        private Group()
        {
        }

        internal Group(string name)
        {
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentException(@"Argument must have a valid value", nameof(name));
        }

        public ScriptedSprite CreateSprite(string path, Anchor origin = Anchor.TopLeft, Vector2 position = default, Layer layer = Layer.Background)
        {
            var sprite = new ScriptedSprite(this, path, layer, position, origin);
            elements.Add(sprite);
            return sprite;
        }

        public ScriptedAnimation CreateAnimation(string path, Anchor origin = Anchor.TopLeft, Vector2 position = default, int frameCount = 0, double frameDelay = 0, AnimationLoopType loopType = AnimationLoopType.LoopForever, Layer layer = Layer.Background)
        {
            var animation = new ScriptedAnimation(this, path, layer, position, origin, frameCount, frameDelay, loopType);
            elements.Add(animation);
            return animation;
        }

        public void CreateSample(string path, double time, int volume = 100, Layer layer = Layer.Background)
            => elements.Add(new ScriptedSample(this, path, time, layer, volume));

        public void CreateVideo(string path, int offset, Vector2 positionOffset = default)
            => elements.Add(new ScriptedVideo(this, path, offset, positionOffset));

        internal void Clear() => elements.Clear();

        private class ScriptedElementComparer : IComparer<IScriptElement>
        {
            public int Compare(IScriptElement x, IScriptElement y)
            {
                int result = x.StartTime.CompareTo(y.StartTime);

                if (result != 0)
                    return result;

                return (x as IScriptElementHasDuration)?.EndTime.CompareTo((y as IScriptElementHasDuration)?.EndTime) ?? 0;
            }
        }
    }
}
