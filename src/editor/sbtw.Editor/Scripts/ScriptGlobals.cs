// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Storyboards;
using osuTK;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts.Assets;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Scripts
{
    public class ScriptGlobals
    {
        public IBeatmap Beatmap;
        public Waveform Waveform;

        internal ICanProvideAssets AssetProvider;
        internal ICanProvideGroups GroupProvider;
        internal ICanProvideFiles FileProvider;
        internal ICanProvideLogger Logger;

        public string GetAsset(string path, Asset asset)
        {
            if (AssetProvider == null)
                throw new NotSupportedException(@"This script does not support getting assets.");

            asset.Path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            AssetProvider.Assets.Add(asset);
            return path;
        }

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

        public string Fetch(string path, bool _)
            => Encoding.Default.GetString(Fetch(path));

        public void Log(object message)
            => Log(message, LogLevel.Debug);

        protected void Log(object message, LogLevel level = LogLevel.Debug)
        {
            if (Logger == null)
                throw new NotSupportedException(@"This script does not support logging.");

            Logger.Log(message, level);
        }

#pragma warning disable CA2211 // see ScriptGlobalsHelper

        public static IReadOnlyList<Type> TYPES { get; } = new[]
        {
            typeof(Text),
            typeof(Rectangle),
            typeof(Layer),
            typeof(Anchor),
            typeof(Easing),
            typeof(Vector2),
            typeof(Colour4),
            typeof(AnimationLoopType),
        };

#pragma warning restore CA2211

    }
}
