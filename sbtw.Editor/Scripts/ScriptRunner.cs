// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sbtw.Editor.Scripts.Elements;
using sbtw.Editor.Storyboards;

namespace sbtw.Editor.Scripts
{
    public abstract class ScriptRunner<T>
        where T : new()
    {
        public async Task<T> GenerateAsync(IEnumerable<Script> scripts, CancellationToken token = default)
        {
            var context = CreateContext();

            PreGenerate(context);

            var groups = (await Task.WhenAll(scripts.Select(s => s.GenerateAsync(token)))).SelectMany(s => s)
                .GroupBy(g => g.Name, g => g.Elements, (key, g) => new ScriptElementGroup(key, g.SelectMany(a => a)));

            foreach (var group in groups)
            {
                token.ThrowIfCancellationRequested();
                foreach (var layer in Enum.GetValues<Layer>())
                {
                    foreach (var element in group.Elements.Where(e => e.Layer == layer).OrderBy(e => e, new ScriptedElementComparer()))
                        handle(context, element);
                }
            }

            PostGenerate(context);

            return context;
        }

        public T Generate(IEnumerable<Script> scripts) => GenerateAsync(scripts).Result;

        private bool videoHandled;

        private void handle(T context, IScriptedElement element)
        {
            switch (element)
            {
                case ScriptedAnimation animation:
                    HandleAnimation(context, animation);
                    break;

                case ScriptedSprite sprite:
                    HandleSprite(context, sprite);
                    break;

                case ScriptedSample sample:
                    HandleSample(context, sample);
                    break;

                case ScriptedVideo video:
                    {
                        if (!videoHandled)
                        {
                            HandleVideo(context, video);
                            videoHandled = true;
                        }
                        break;
                    }
            }
        }

        protected abstract T CreateContext();

        protected virtual void PreGenerate(T context)
        {
        }

        protected virtual void PostGenerate(T context)
        {
        }

        protected abstract void HandleAnimation(T context, ScriptedAnimation animation);

        protected abstract void HandleSprite(T context, ScriptedSprite sprite);

        protected abstract void HandleSample(T context, ScriptedSample sample);

        protected abstract void HandleVideo(T context, ScriptedVideo video);

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
