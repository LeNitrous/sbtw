// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using sbtw.Common.Scripting;
using sbtw.Game.Projects;

namespace sbtw.Game.Scripting
{
    public abstract class ScriptAssemblyRunner<T> : IDisposable
        where T : new()
    {
        private readonly ScriptAssemblyContext assemblyContext;
        private readonly string outputPath;
        private IEnumerable<ScriptElementGroup> groups;

        /// <summary>
        /// The types loaded from the assembly.
        /// </summary>
        public readonly IReadOnlyList<Type> Loaded;

        /// <summary>
        /// Gets whether this runner is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        protected ScriptAssemblyRunner(Project project)
        {
            outputPath = Path.Combine(project.Path, "bin", "Debug", "net5.0");
            assemblyContext = new ScriptAssemblyContext(outputPath);

            var assembly = assemblyContext.LoadFromAssemblyPath(Path.Combine(outputPath, Path.ChangeExtension(project.Name, ".dll")));
            Loaded = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Script)) && !t.IsAbstract).ToArray();
        }

        /// <summary>
        /// Generates <see cref="T"/> with the option of ordering groups.
        /// </summary>
        public T Generate(IEnumerable<string> ordering = null)
        {
            if (IsDisposed)
                throw new InvalidOperationException("Cannot generate an already disposed runner.");

            var context = CreateContext();

            PreGenerate(context);

            // Merge all elements and group them by name
            // TODO: Make this non-blocking!
            groups = Loaded
                .SelectMany(type => { var script = Activator.CreateInstance(type) as Script; script.Generate(); return script.Groups; })
                .GroupBy(g => g.Name, g => g.Elements, (key, g) => new ScriptElementGroup(key) { Elements = g.SelectMany(a => a) });

            // If ordering is specified, order them by key index
            if (ordering != null && ordering.Any())
            {
                var lookup = ordering.Select((key, idx) => new { key, idx }).ToDictionary(p => p.key, p => p.idx);
                groups = groups.OrderBy(g => lookup.ContainsKey(g.Name) ? lookup[g.Name] : 0);
            }

            // Generate...
            foreach (var group in groups)
            {
                // Iterate over each layer of the storyboard
                foreach (var layer in Enum.GetValues<Layer>())
                {
                    // Get all elements of that layer then order them by start time (or end time if applicable)
                    foreach (var element in group.Elements.Where(e => e.Layer == layer).OrderBy(e => e, new ScriptedStoryboardElementComparer()))
                        handle(context, element);
                }
            }

            PostGenerate(context);

            return context;
        }

        /// <summary>
        /// Generates <see cref="T"/> with the option of ordering groups and returning a list of added and removed group names.
        /// </summary>
        public T Generate(IEnumerable<string> ordering, out IEnumerable<string> added, out IEnumerable<string> removed)
        {
            var context = Generate(ordering);
            var groupNames = groups.Select(g => g.Name);
            added = groupNames.Except(ordering);
            removed = ordering.Except(groupNames);
            return context;
        }

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
                    HandleVideo(context, video);
                    break;
            }
        }

        /// <summary>
        /// Called before the element generation process.
        /// </summary>
        protected virtual void PreGenerate(T context)
        {
        }

        /// <summary>
        /// Called after the element generation process.
        /// </summary>
        protected virtual void PostGenerate(T context)
        {
        }

        /// <summary>
        /// Creates <see cref="T"/> passed when handling <see cref="IScriptedElement"/>
        /// </summary>
        protected abstract T CreateContext();

        /// <summary>
        /// Called when a storyboard animation has to be handled.
        /// </summary>
        protected abstract void HandleAnimation(T context, ScriptedAnimation animation);

        /// <summary>
        /// Called when a storyboard sprite has to be handled.
        /// </summary>
        protected abstract void HandleSprite(T context, ScriptedSprite sprite);

        /// <summary>
        /// Called when a storyboard sample has to be handled.
        /// </summary>
        protected abstract void HandleSample(T context, ScriptedSample sample);

        /// <summary>
        /// Called when a storyboard video has to be handled.
        /// </summary>
        protected abstract void HandleVideo(T context, ScriptedVideo video);

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                assemblyContext.Unload();
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private class ScriptedStoryboardElementComparer : IComparer<IScriptedElement>
        {
            public int Compare(IScriptedElement x, IScriptedElement y)
            {
                int result = (x as IScriptedElementHasStartTime)?.StartTime.CompareTo((y as IScriptedElementHasStartTime)?.StartTime) ?? 0;

                if (result != 0)
                    return result;

                return (x as IScriptedElementHasEndTime)?.EndTime.CompareTo((y as IScriptedElementHasEndTime)?.EndTime) ?? 0;
            }
        }
    }
}
