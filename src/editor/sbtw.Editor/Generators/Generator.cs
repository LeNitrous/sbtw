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
    public abstract class Generator<TResult, TElement>
    {
        public TResult Generate(IProject project, Dictionary<string, object> resources = null, ExportTarget? target = null, bool includeHidden = true)
            => GenerateAsync(project, resources, target, includeHidden).Result;

        public async Task<TResult> GenerateAsync(IProject project, Dictionary<string, object> resources = null, ExportTarget? target = null, bool includeHidden = true, CancellationToken token = default)
        {
            if (project is not ICanProvideScripts scriptsProvider)
                throw new ArgumentException("Project does not support managing scripts.", nameof(project));

            if (project is not ICanProvideGroups groupsProvider)
                throw new ArgumentException("Project does not support providing groups.", nameof(project));

            var context = CreateContext();

            PreGenerate(context);

            IEnumerable<Group> groups = groupsProvider.Groups;

            foreach (var group in groups)
                group.Clear();

            var scripts = await scriptsProvider.Scripts.GetScriptsAsync(resources, token);

            foreach (var script in scripts)
            {
                token.ThrowIfCancellationRequested();
                await script.ExecuteAsync(token);
            }

            if (target.HasValue && target.Value != ExportTarget.None)
                groups = groups.Where(g => g.Target.Value == target.Value);

            if (!includeHidden)
                groups = groups.Where(g => g.Visible.Value);

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

            return context;
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
