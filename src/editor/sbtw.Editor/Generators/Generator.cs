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
        public T Generate(IEnumerable<IScript> scripts, ICanProvideGroups groupsProvider)
            => GenerateAsync(scripts, groupsProvider).Result;

        public async Task<T> GenerateAsync(IEnumerable<IScript> scripts, ICanProvideGroups groupsProvider, CancellationToken token = default)
        {
            if (scripts == null)
                throw new ArgumentNullException(nameof(scripts));

            if (groupsProvider == null)
                throw new ArgumentNullException(nameof(groupsProvider));

            var context = CreateContext();

            PreGenerate(context);

            foreach (var script in scripts)
            {
                token.ThrowIfCancellationRequested();
                await script.ExecuteAsync(token);
            }

            foreach (var group in groupsProvider.Groups)
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

            return context;
        }

        protected virtual void PreGenerate(T context)
        {
        }

        protected virtual void PostGenerate(T context)
        {
        }

        protected abstract T CreateContext();
        protected abstract U CreateAnimation(T context, ScriptedAnimation animation);
        protected abstract U CreateSample(T context, ScriptedSample sample);
        protected abstract U CreateSprite(T context, ScriptedSprite sprite);
        protected abstract U CreateVideo(T context, ScriptedVideo video);

        private U create(T context, IScriptElement element)
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
