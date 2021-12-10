// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Text;
using sbtw.Common.Scripting;
using sbtw.Game.Projects;

namespace sbtw.Game.Scripting
{
    public class StoryboardEncoder : ScriptRunner<StringBuilder>
    {
        private readonly Dictionary<Layer, StringBuilder> layers = new Dictionary<Layer, StringBuilder>();

        public StoryboardEncoder(Project project)
            : base(project)
        {
        }

        protected override StringBuilder CreateContext() => new StringBuilder();

        protected override void PreGenerate(StringBuilder context)
        {
            context.AppendLine("[Events]");
            context.AppendLine("// Background and Video Events");

            layers.Clear();

            foreach (var layer in Enum.GetValues<Layer>())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"// Storyboard Layer {layer} {Enum.GetName(layer)}");
                layers.Add(layer, builder);
            }
        }

        protected override void HandleAnimation(StringBuilder context, ScriptedAnimation animation)
            => layers[animation.Layer].AppendLine(animation.Encode());

        protected override void HandleSample(StringBuilder context, ScriptedSample sample)
            => layers[sample.Layer].AppendLine(sample.Encode());

        protected override void HandleSprite(StringBuilder context, ScriptedSprite sprite)
            => layers[sprite.Layer].AppendLine(sprite.Encode());

        protected override void HandleVideo(StringBuilder context, ScriptedVideo video)
            => context.AppendLine(video.Encode());

        protected override void PostGenerate(StringBuilder context)
        {
            foreach ((var _, var builder) in layers)
                context.AppendLine(builder.ToString());
        }
    }
}
