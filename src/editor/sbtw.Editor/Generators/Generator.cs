// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Generators
{
    public abstract class Generator<T, U>
    {
        public async Task<GeneratorResult<T, U>> GenerateAsync(GeneratorConfig config, CancellationToken token = default)
        {
            var context = CreateContext();

            PreGenerate(context);

            var elements = new Dictionary<IScriptedElement, U>();
            var ordering = config.Ordering?.ToArray() ?? Array.Empty<string>();
            var generated = await Task.WhenAll(config.Scripts.Select(s => apply(s, config.Variables?.GetValueOrDefault(s.Name), token)));

            var groups = generated
                .SelectMany(r => r.Groups)
                .GroupBy(k => k.Name, v => v.Elements, (k, v) => new ScriptElementGroup(k, v.SelectMany(a => a)))
                .OrderBy(g => Array.IndexOf(ordering, g.Name));

            foreach (var group in groups)
            {
                token.ThrowIfCancellationRequested();
                foreach (var layer in Enum.GetValues<Layer>())
                {
                    foreach (var element in group.Elements.Where(e => e.Layer == layer).OrderBy(e => e, new ScriptedElementComparer()))
                        elements.TryAdd(element, create(context, element));
                }
            }

            PostGenerate(context);

            return new GeneratorResult<T, U>
            {
                Elements = elements,
                Result = context,
                Groups = groups.Select(g => g.Name),
                Variables = generated.ToDictionary(k => k.Name, v => (IReadOnlyDictionary<string, object>)v.Variables.ToDictionary(i => i.Name, j => j.Value)),
            };
        }

        public GeneratorResult<T, U> Generate(GeneratorConfig config) => GenerateAsync(config).Result;

        protected abstract T CreateContext();
        protected abstract U CreateAnimation(T context, ScriptedAnimation animation);
        protected abstract U CreateSample(T context, ScriptedSample sample);
        protected abstract U CreateSprite(T context, ScriptedSprite sprite);
        protected abstract U CreateVideo(T context, ScriptedVideo video);

        protected virtual void PreGenerate(T context)
        {
        }

        protected virtual void PostGenerate(T context)
        {
        }

        private U create(T context, IScriptedElement element)
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

        private static Task<ScriptGenerationResult> apply(Script script, IReadOnlyDictionary<string, object> variables = null, CancellationToken token = default)
        {
            if (variables != null)
            {
                foreach ((string name, object value) in variables)
                    script.SetInternal(name, value);
            }

            return script.GenerateAsync(token);
        }

        private class ScriptedElementComparer : IComparer<IScriptedElement>
        {
            public int Compare(IScriptedElement x, IScriptedElement y)
            {
                int result = x.StartTime.CompareTo(y.StartTime);

                if (result != 0)
                    return result;

                return (x as IScriptedElementWithDuration)?.EndTime.CompareTo((y as IScriptedElementWithDuration)?.EndTime) ?? 0;
            }
        }
    }

    public abstract class Generator<T> : Generator<T, T>
    {
    }
}
