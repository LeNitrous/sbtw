// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Textures;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Skinning;

namespace sbtw.Game.Projects
{
    /// <summary>
    /// A <see cref="WorkingBeatmap"/> shim that completely bypasses <see cref="FileStore"/> allowing direct access to the file system.
    /// </summary>
    public class ProjectWorkingBeatmap : WorkingBeatmap
    {
        private readonly IBeatmap beatmap;
        private readonly GameHost host;
        private readonly IBeatmapResourceProvider resources;

        public ProjectWorkingBeatmap(IBeatmapResourceProvider resources, BeatmapInfo beatmapInfo, IBeatmap beatmap, GameHost host, AudioManager audio)
            : base(beatmapInfo, audio)
        {
            this.host = host;
            this.beatmap = beatmap;
            this.resources = resources;
        }

        public IResourceStore<TextureUpload> CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
            => host.CreateTextureLoaderStore(underlyingStore);

        public override Stream GetStream(string storagePath)
            => resources.Files.GetStream(storagePath);

        protected override Texture GetBackground()
            => resources.LargeTextureStore.Get(BeatmapSetInfo.GetPathForFile(Metadata.BackgroundFile));

        protected override IBeatmap GetBeatmap()
            => beatmap;

        protected override Track GetBeatmapTrack()
            => resources.Tracks.Get(BeatmapSetInfo.GetPathForFile(Metadata.AudioFile));

        protected override ISkin GetSkin()
            => new ProjectLegacyBeatmapSkin(BeatmapInfo, resources.Resources, resources);

        private class ProjectLegacyBeatmapSkin : LegacySkin
        {
            protected override bool AllowManiaSkin => false;

            protected override bool UseCustomSampleBanks => true;

            public ProjectLegacyBeatmapSkin(BeatmapInfo beatmapInfo, IResourceStore<byte[]> storage, IStorageResourceProvider resources)
                : base(createSkinInfo(beatmapInfo), storage, resources, beatmapInfo.Path)
            {
            }

            public override Drawable GetDrawableComponent(ISkinComponent component)
            {
                if (component is SkinnableTargetComponent targetComponent)
                {
                    switch (targetComponent.Target)
                    {
                        case SkinnableTarget.MainHUDComponents:
                            if (!this.HasFont(LegacyFont.Score))
                                return null;
                            break;
                    }
                }

                return base.GetDrawableComponent(component);
            }

            public override IBindable<TValue> GetConfig<TLookup, TValue>(TLookup lookup)
            {
                switch (lookup)
                {
                    case SkinConfiguration.LegacySetting setting when setting == SkinConfiguration.LegacySetting.Version:
                        return null;
                }

                return base.GetConfig<TLookup, TValue>(lookup);
            }

            public override Texture GetTexture(string componentName, WrapMode wrapModeS, WrapMode wrapModeT)
            {
                foreach (var name in getFallbackNames(componentName).SelectMany(getTextureFallbackNames))
                {
                    float ratio = 2;
                    var texture = Textures?.Get($"{name}@2x", wrapModeS, wrapModeT);

                    if (texture == null)
                    {
                        ratio = 1;
                        texture = Textures?.Get(name, wrapModeS, wrapModeT);
                    }

                    if (texture == null)
                        continue;

                    texture.ScaleAdjust = ratio;
                    return texture;
                }

                return null;
            }

            private IEnumerable<string> getTextureFallbackNames(string componentName)
            {
                yield return Path.ChangeExtension(componentName, ".png");
                yield return Path.ChangeExtension(componentName, ".jpg");
                yield return Path.ChangeExtension(componentName, ".jpeg");
            }

            public override ISample GetSample(ISampleInfo sampleInfo)
            {
                IEnumerable<string> lookupNames;

                if (sampleInfo is HitSampleInfo hitSample)
                {
                    lookupNames = getLegacyLookupNames(hitSample);
                }
                else
                {
                    lookupNames = sampleInfo.LookupNames.SelectMany(getFallbackNames);
                }

                foreach (var lookup in lookupNames)
                {
                    var sample = Samples?.Get(Path.ChangeExtension(lookup, ".wav"));

                    if (sample != null)
                        return sample;
                }

                return null;
            }

            private IEnumerable<string> getLegacyLookupNames(HitSampleInfo hitSample)
            {
                var lookupNames = hitSample.LookupNames.SelectMany(getFallbackNames);

                if (!UseCustomSampleBanks && !string.IsNullOrEmpty(hitSample.Suffix))
                {
                    lookupNames = lookupNames.Where(name => !name.EndsWith(hitSample.Suffix, StringComparison.Ordinal));
                }

                foreach (var l in lookupNames)
                    yield return l;

                yield return hitSample.Name;
            }

            private IEnumerable<string> getFallbackNames(string componentName)
            {
                yield return componentName;
                string lastPiece = componentName.Split('/').Last();
                yield return componentName.StartsWith("Gameplay/taiko/", StringComparison.Ordinal) ? "taiko-" + lastPiece : lastPiece;
            }

            protected override IBindable<osuTK.Graphics.Color4> GetComboColour(IHasComboColours source, int index, IHasComboInformation combo)
                => base.GetComboColour(source, combo.ComboIndexWithOffsets, combo);

            private static SkinInfo createSkinInfo(BeatmapInfo beatmapInfo) =>
                new SkinInfo { Name = beatmapInfo.ToString(), Creator = beatmapInfo.Metadata?.Author.Username };
        }
    }
}
