// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using osuTK;
using sbtw.Editor.Scripts.Commands;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Generators.Steps
{
    public class RoundToPrecisionStep : ProcessGroupStep
    {
        private readonly int precisionMove;
        private readonly int precisionScale;
        private readonly int precisionAlpha;
        private readonly int precisionRotation;

        public RoundToPrecisionStep(int movePrecision = 4, int scalePrecision = 4, int alphaPrecision = 4, int rotationPrecision = 4)
        {
            precisionMove = movePrecision;
            precisionScale = scalePrecision;
            precisionAlpha = alphaPrecision;
            precisionRotation = rotationPrecision;
        }

        protected override Task ProcessElement(IScriptElement element, CancellationToken token)
        {
            if (element is ScriptedSprite sprite)
                sprite.Position = new Vector2(MathF.Round(sprite.Position.X, precisionMove), MathF.Round(sprite.Position.Y, precisionMove));

            return Task.CompletedTask;
        }

        protected override Task ProcessTimeline(IScriptCommandTimelineGroup timeline, CancellationToken token)
        {
            foreach (var command in timeline.X.Commands)
            {
                command.StartValue = MathF.Round(command.StartValue, precisionMove);
                command.EndValue = MathF.Round(command.EndValue, precisionMove);
            }

            foreach (var command in timeline.Y.Commands)
            {
                command.StartValue = MathF.Round(command.StartValue, precisionMove);
                command.EndValue = MathF.Round(command.EndValue, precisionMove);
            }

            foreach (var command in timeline.Alpha.Commands)
            {
                command.StartValue = MathF.Round(command.StartValue, precisionAlpha);
                command.EndValue = MathF.Round(command.EndValue, precisionAlpha);
            }

            foreach (var command in timeline.Rotation.Commands)
            {
                command.StartValue = MathF.Round(command.StartValue, precisionRotation);
                command.EndValue = MathF.Round(command.EndValue, precisionRotation);
            }

            foreach (var command in timeline.Scale.Commands)
            {
                command.StartValue = MathF.Round(command.StartValue, precisionScale);
                command.EndValue = MathF.Round(command.EndValue, precisionScale);
            }

            foreach (var command in timeline.Move.Commands)
            {
                command.StartValue = new Vector2(MathF.Round(command.StartValue.X, precisionMove), MathF.Round(command.StartValue.Y, precisionMove));
                command.EndValue = new Vector2(MathF.Round(command.EndValue.X, precisionMove), MathF.Round(command.EndValue.Y, precisionMove));
            }

            foreach (var command in timeline.VectorScale.Commands)
            {
                command.StartValue = new Vector2(MathF.Round(command.StartValue.X, precisionScale), MathF.Round(command.StartValue.Y, precisionScale));
                command.EndValue = new Vector2(MathF.Round(command.EndValue.X, precisionScale), MathF.Round(command.EndValue.Y, precisionScale));
            }

            return Task.CompletedTask;
        }
    }
}
