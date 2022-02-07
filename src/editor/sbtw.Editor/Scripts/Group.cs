// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Lists;
using osu.Game.Storyboards;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// Represents a collection of elements.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The name of the group.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The <see cref="IScript"/> that owns this group.
        /// </summary>
        internal readonly IScript Owner;

        /// <summary>
        /// The <see cref="GroupCollection"/> this group is provided by.
        /// </summary>
        internal readonly GroupCollection Provider;

        /// <summary>
        /// Whether this group should be visible in the editor.
        /// </summary>
        internal readonly BindableBool Visible = new BindableBool(true);

        /// <summary>
        /// Determines where this group should be exported to.
        /// </summary>
        internal readonly Bindable<ExportTarget> Target = new Bindable<ExportTarget>(ExportTarget.Storyboard);

        /// <summary>
        /// Gets the elements this group has.
        /// </summary>
        public IReadOnlyList<IScriptElement> Elements => elements;

        private readonly SortedList<IScriptElement> elements = new SortedList<IScriptElement>(new ScriptedElementComparer());

        public Group(IScript owner, GroupCollection provider, string name)
        {
            Name = name;
            Owner = owner;
            Provider = provider;
            Provider.Add(this);
        }

        public ScriptedSprite CreateSprite(string path, Anchor origin = Anchor.TopLeft, Vector2 position = default, Layer layer = Layer.Background)
        {
            var sprite = new ScriptedSprite(Owner, this, path, layer, position, origin);
            elements.Add(sprite);
            return sprite;
        }

        public ScriptedAnimation CreateAnimation(string path, Anchor origin = Anchor.TopLeft, Vector2 position = default, int frameCount = 0, double frameDelay = 0, AnimationLoopType loopType = AnimationLoopType.LoopForever, Layer layer = Layer.Background)
        {
            var animation = new ScriptedAnimation(Owner, this, path, layer, position, origin, frameCount, frameDelay, loopType);
            elements.Add(animation);
            return animation;
        }

        public void CreateSample(string path, double time, int volume = 100, Layer layer = Layer.Background)
            => elements.Add(new ScriptedSample(Owner, this, path, time, layer, volume));

        public void CreateVideo(string path, int offset, Vector2 positionOffset = default)
            => elements.Add(new ScriptedVideo(Owner, this, path, offset, positionOffset));

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
