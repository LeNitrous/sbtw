// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Threading;
using System.Threading.Tasks;
using osuTK;
using sbtw.Editor.Scripts.Commands;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Generators.Steps
{
    public class OffsetForWidescreenStep : ProcessGroupStep
    {
        protected override Task ProcessElement(IScriptElement element, CancellationToken token)
        {
            if (element is ScriptedSprite sprite)
                sprite.Position = Vector2.Subtract(sprite.Position, new Vector2(107, 0));

            return Task.CompletedTask;
        }

        protected override Task ProcessTimeline(IScriptCommandTimelineGroup timeline, CancellationToken token)
        {
            foreach (var command in timeline.X.Commands)
            {
                command.StartValue -= 107;
                command.EndValue -= 107;
            }

            foreach (var command in timeline.Move.Commands)
            {
                command.StartValue = Vector2.Subtract(command.StartValue, new Vector2(107, 0));
                command.EndValue = Vector2.Subtract(command.EndValue, new Vector2(107, 0));
            }

            return Task.CompletedTask;
        }
    }
}
