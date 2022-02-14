// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Scripts.Commands;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Generators.Steps
{
    public abstract class ProcessGroupStep : GeneratorStep
    {
        public sealed override async Task<GeneratorContext> PostProcess(GeneratorContext context, CancellationToken token)
        {
            foreach (var element in context.Groups.SelectMany(g => g.Elements))
            {
                await ProcessElement(element, token);

                if (element is not ScriptedSprite sprite)
                    continue;

                var timelines = sprite.Loops.Cast<IScriptCommandTimelineGroup>()
                    .Concat(sprite.Triggers.Cast<IScriptCommandTimelineGroup>())
                    .Append(sprite.Timeline);

                foreach (var timeline in timelines)
                    await ProcessTimeline(timeline, token);
            }

            return context;
        }

        protected abstract Task ProcessElement(IScriptElement element, CancellationToken token);
        protected abstract Task ProcessTimeline(IScriptCommandTimelineGroup timeline, CancellationToken token);
    }
}
