// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Generators.Steps;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Generators
{
    public abstract class Generator<TResult, TElement>
    {
        protected readonly ICanProvideScripts Provider;
        private readonly Queue<GeneratorStep> steps = new Queue<GeneratorStep>();

        public Generator(ICanProvideScripts provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public Generator<TResult, TElement> AddStep(GeneratorStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            steps.Enqueue(step);
            return this;
        }

        public GeneratorResult<TResult> Generate(ScriptGlobals globals)
            => GenerateAsync(globals).Result;

        public async Task<GeneratorResult<TResult>> GenerateAsync(ScriptGlobals globals, CancellationToken token = default)
        {
            if (globals == null)
                throw new ArgumentNullException(nameof(globals));

            if (globals.GroupProvider == null)
                throw new ArgumentException($"{nameof(ScriptGlobals)} must provide groups.");

            var generatorContext = CreateContext();
            var stepContext = new GeneratorContext { Groups = globals.GroupProvider.Groups };

            foreach (var group in stepContext.Groups)
                group.Clear();

            PreGenerate(generatorContext);

            foreach (var step in steps)
                stepContext = await step.PreProcess(stepContext, token);

            var scripts = await Provider.Scripts.ExecuteAsync(globals, token);

            foreach (var group in stepContext.Groups.ToArray())
            {
                if (!group.Elements.Any())
                    globals.GroupProvider.Groups.Remove(group);
            }

            foreach (var step in steps)
                stepContext = await step.PostProcess(stepContext, token);

            foreach (var step in steps)
                stepContext = await step.PreGenerate(stepContext, token);

            foreach (var group in stepContext.Groups)
            {
                foreach (var layer in Enum.GetValues<Layer>())
                {
                    foreach (var element in group.Elements.Where(e => e.Layer == layer))
                    {
                        token.ThrowIfCancellationRequested();
                        create(generatorContext, element);
                    }
                }
            }

            foreach (var step in steps)
                stepContext = await step.PostGenerate(stepContext, token);

            PostGenerate(generatorContext);

            return new GeneratorResult<TResult> { Result = generatorContext, Scripts = scripts };
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
