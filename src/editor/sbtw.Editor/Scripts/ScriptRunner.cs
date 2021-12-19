// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    public abstract class ScriptRunner<TResult, TGenerated>
        where TResult : new()
    {
        public async Task<ScriptRunnerGenerationResult<TResult, TGenerated>> GenerateAsync(ScriptRunnerGenerationConfiguration config, CancellationToken token = default)
        {
            var context = CreateContext();

            PreGenerate(context);

            var map = new Dictionary<IScriptedElement, TGenerated>();
            var generated = await Task.WhenAll(config.Scripts.Select(s => apply(s, config.Variables?.GetValueOrDefault(s.Name), token)));

            var ordering = config.Ordering?.ToArray() ?? Array.Empty<string>();
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
                        map.TryAdd(element, handle(context, element));
                }
            }

            PostGenerate(context);

            return new ScriptRunnerGenerationResult<TResult, TGenerated>
            {
                Map = map,
                Result = context,
                Groups = groups.Select(g => g.Name),
                Variables = generated.ToDictionary(k => k.Name, v => (IReadOnlyDictionary<string, object>)v.Variables.ToDictionary(i => i.Name, j => j.Value)),
            };
        }

        public ScriptRunnerGenerationResult<TResult, TGenerated> Generate(ScriptRunnerGenerationConfiguration config)
            => GenerateAsync(config).Result;

        private static Task<ScriptGenerationResult> apply(Script script, IReadOnlyDictionary<string, object> variables = null, CancellationToken token = default)
        {
            if (variables != null)
            {
                foreach ((string name, object value) in variables)
                    script.SetInternal(name, value);
            }

            return script.GenerateAsync(token);
        }

        private bool videoHandled;

        private TGenerated handle(TResult context, IScriptedElement element)
        {
            switch (element)
            {
                case ScriptedAnimation animation:
                    return HandleAnimation(context, animation);

                case ScriptedSprite sprite:
                    return HandleSprite(context, sprite);

                case ScriptedSample sample:
                    return HandleSample(context, sample);

                case ScriptedVideo video:
                    {
                        if (!videoHandled)
                        {
                            videoHandled = true;
                            return HandleVideo(context, video);
                        }

                        return default;
                    }

                default:
                    return default;
            }
        }

        protected abstract TResult CreateContext();

        protected virtual void PreGenerate(TResult context)
        {
        }

        protected virtual void PostGenerate(TResult context)
        {
        }

        protected abstract TGenerated HandleAnimation(TResult context, ScriptedAnimation animation);
        protected abstract TGenerated HandleSprite(TResult context, ScriptedSprite sprite);
        protected abstract TGenerated HandleSample(TResult context, ScriptedSample sample);
        protected abstract TGenerated HandleVideo(TResult context, ScriptedVideo video);

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
}
