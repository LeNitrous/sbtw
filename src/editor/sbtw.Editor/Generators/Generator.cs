// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Scripts.Types;

namespace sbtw.Editor.Generators
{
    public abstract class Generator<T, U>
    {
        public async Task<GeneratorResult<T, U>> GenerateAsync(GeneratorConfig config, CancellationToken token = default)
        {
            var context = CreateContext();

            PreGenerate(context);

            var elements = new Dictionary<string, List<U>>();
            var ordering = config.Groups?.ToArray() ?? Enumerable.Empty<GroupSetting>();
            var generated = await Task.WhenAll(config.Scripts?.Select(s => s.GenerateAsync(config.Storage, config.Beatmap, config.Waveform, token)));
            var performed = generated.Where(g => g.Exception == null);

            var groups = performed
                .SelectMany(r => r.Groups)
                .GroupBy(k => k.Name, v => v.Elements, (k, v) => new ScriptElementGroup(k, v.SelectMany(a => a)))
                .OrderBy(g => Array.IndexOf(ordering.Reverse().Select(g => g.Name).ToArray(), g.Name));

            foreach (var group in groups)
            {
                token.ThrowIfCancellationRequested();
                foreach (var layer in Enum.GetValues<Layer>())
                {
                    if (!elements.TryGetValue(group.Name, out var list))
                        elements.Add(group.Name, list = new List<U>());

                    foreach (var element in group.Elements.Where(e => e.Layer == layer).OrderBy(e => e, new ScriptedElementComparer()))
                    {
                        if (!(ordering.FirstOrDefault(o => o.Name == group.Name)?.Hidden.Value ?? false))
                            list.Add(create(context, element));
                    }
                }
            }

            PostGenerate(context);

            return new GeneratorResult<T, U>
            {
                Result = context,
                Assets = performed.SelectMany(g => g.Assets),
                Groups = elements,
                Scripts = generated,
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
