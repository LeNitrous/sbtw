// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Audio.Track;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Generators
{
    public abstract class Generator<T, U>
    {
        public async Task<GeneratorResult<T, U>> GenerateAsync(GeneratorConfig config, CancellationToken token = default)
        {
            var scriptNames = config.Scripts.Select(s => s.Name);
            if (scriptNames.Count() != scriptNames.Distinct().Count())
                throw new ArgumentException($"Generator {nameof(config)} has duplicate script names");

            var context = CreateContext();

            PreGenerate(context);

            var elements = new Dictionary<IScriptedElement, U>();
            var ordering = config.Ordering?.ToArray() ?? Array.Empty<string>();
            var generated = await Task.WhenAll(config.Scripts?.Select(s =>
                apply(s, config.Storage, config.Beatmap, config.Waveform, config.Variables?.GetValueOrDefault(s.Name), token))
                ?? Array.Empty<Task<ScriptGenerationResult>>()
            );

            foreach (var script in config.Scripts)
                script.Dispose();

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
                Faulted = generated.Where(s => s.Faulted).Select(s => s.Name),
                Variables = generated.ToDictionary(k => k.Name, v => v.Variables),
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

        private static Task<ScriptGenerationResult> apply(Script script, Storage storage, IBeatmap beatmap, Waveform waveform, IEnumerable<ScriptVariableInfo> variables = null, CancellationToken token = default)
        {
            if (variables != null)
            {
                foreach (var variable in variables)
                {
                    script.SetValueInternal(variable.Name, variable.Value);
                }
            }

            return script.GenerateAsync(storage, beatmap, waveform, token);
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