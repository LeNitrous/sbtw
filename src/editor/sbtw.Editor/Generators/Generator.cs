// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Commands;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Generators
{
    public abstract class Generator<TResult, TElement>
    {
        protected readonly IProject Project;
        private readonly int precisionMove = 4;
        private readonly int precisionScale = 4;
        private readonly int precisionAlpha = 4;
        private readonly int precisionRotation = 4;

        public Generator(IProject project)
        {
            if (project is not ICanProvideScripts)
                throw new ArgumentException("Project does not support managing scripts.", nameof(project));

            Project = project;

            if (project is not IGeneratorConfig config)
                return;

            precisionMove = config.PrecisionMove.Value;
            precisionScale = config.PrecisionScale.Value;
            precisionAlpha = config.PrecisionAlpha.Value;
            precisionRotation = config.PrecisionRotation.Value;
        }

        public GeneratorResult<TResult> Generate(ScriptGlobals globals = null, ExportTarget? target = null, bool includeHidden = true)
            => GenerateAsync(globals, target, includeHidden).Result;

        public async Task<GeneratorResult<TResult>> GenerateAsync(ScriptGlobals globals = null, ExportTarget? target = null, bool includeHidden = true, CancellationToken token = default)
        {
            var scriptProvider = Project as ICanProvideScripts;

            var context = CreateContext();

            IEnumerable<Group> groups = globals.GroupProvider.Groups ?? Enumerable.Empty<Group>();

            foreach (var group in groups)
                group.Clear();

            PreGenerate(context);

            var scripts = await scriptProvider.Scripts.ExecuteAsync(globals, token);

            foreach (var group in groups.ToArray())
            {
                if (!group.Elements.Any())
                    globals.GroupProvider.Groups.Remove(group);
            }

            if (target.HasValue && target.Value != ExportTarget.None)
                groups = groups.Where(g => g.Target.Value == target.Value);

            if (!includeHidden)
                groups = groups.Where(g => g.Visible.Value);

            if (Project is IGeneratorConfig config && config.UseWidescreen.Value)
                offsetForWideScreen(groups);

            roundFloatsToPrecision(groups);

            foreach (var group in groups)
            {
                foreach (var layer in Enum.GetValues<Layer>())
                {
                    foreach (var element in group.Elements.Where(e => e.Layer == layer))
                    {
                        token.ThrowIfCancellationRequested();
                        create(context, element);
                    }
                }
            }

            PostGenerate(context);

            return new GeneratorResult<TResult> { Result = context, Scripts = scripts };
        }

        protected virtual void PreGenerate(TResult context)
        {
        }

        protected virtual void PostGenerate(TResult context)
        {
        }

        protected abstract TResult CreateContext();
        protected abstract TElement CreateAnimation(TResult context, ScriptedAnimation animation);
        protected abstract TElement CreateSample(TResult context, ScriptedSample sample);
        protected abstract TElement CreateSprite(TResult context, ScriptedSprite sprite);
        protected abstract TElement CreateVideo(TResult context, ScriptedVideo video);

        private static void offsetForWideScreen(IEnumerable<Group> groups)
        {
            foreach (var element in groups.SelectMany(g => g.Elements))
            {
                if (element is not ScriptedSprite sprite)
                    continue;

                sprite.Position = Vector2.Subtract(sprite.Position, new Vector2(107, 0));
            }

            performOnTimelines(groups, timeline =>
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
            });
        }

        private void roundFloatsToPrecision(IEnumerable<Group> groups)
        {
            performOnTimelines(groups, timeline =>
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
                    command.StartValue = new Vector2(MathF.Round(command.StartValue.X), MathF.Round(command.StartValue.Y));
                    command.EndValue = new Vector2(MathF.Round(command.EndValue.X), MathF.Round(command.EndValue.Y));
                }

                foreach (var command in timeline.VectorScale.Commands)
                {
                    command.StartValue = new Vector2(MathF.Round(command.StartValue.X), MathF.Round(command.StartValue.Y));
                    command.EndValue = new Vector2(MathF.Round(command.EndValue.X), MathF.Round(command.EndValue.Y));
                }
            });
        }

        private static void performOnTimelines(IEnumerable<Group> groups, Action<IScriptCommandTimelineGroup> action)
        {
            foreach (var element in groups.SelectMany(g => g.Elements))
            {
                if (element is not ScriptedSprite sprite)
                    continue;

                var timelines = sprite.Loops.Cast<IScriptCommandTimelineGroup>()
                    .Concat(sprite.Triggers.Cast<IScriptCommandTimelineGroup>())
                    .Append(sprite.Timeline);

                foreach (var timeline in timelines)
                    action.Invoke(timeline);
            }
        }

        private TElement create(TResult context, IScriptElement element)
        {
            switch (element)
            {
                case ScriptedAnimation animation:
                    return CreateAnimation(context, animation);

                case ScriptedSprite sprite:
                    return CreateSprite(context, sprite);

                case ScriptedSample sample:
                    return CreateSample(context, sample);

                case ScriptedVideo video:
                    return CreateVideo(context, video);

                default:
                    return default;
            }
        }
    }
}
