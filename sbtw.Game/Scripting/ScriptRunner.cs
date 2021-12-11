// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Python.Runtime;
using sbtw.Common.Scripting;
using sbtw.Game.Projects;
using sbtw.Game.Utils;

namespace sbtw.Game.Scripting
{
    public abstract class ScriptRunner<T> : IDisposable
        where T : new()
    {
        private readonly List<Script> loaded = new List<Script>();
        private readonly ScriptAssemblyContext assemblyContext;

        public bool IsDisposed { get; private set; }

        public ScriptRunner(Project project, V8ScriptEngine jsScriptEngine = null)
        {
            string assemblyOutputPath = Path.Combine(project.Path, "bin", "Debug", "net5.0");
            assemblyContext = new ScriptAssemblyContext(assemblyOutputPath);

            string assemblyPath = Path.Combine(assemblyOutputPath, Path.ChangeExtension(project.Name, ".dll"));
            if (File.Exists(assemblyPath))
            {
                // Load all .NET Scripts
                var assembly = assemblyContext.LoadFromAssemblyPath(assemblyPath);
                foreach (var type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Script)) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null))
                    loaded.Add(Activator.CreateInstance(type) as Script);
            }

            var jsScriptPaths = Directory.GetFiles(project.Path).Where(p => Path.GetExtension(p) == ".js");
            if (jsScriptPaths.Any() && jsScriptEngine != null)
            {
                // Load all JS scripts
                foreach (var scriptPath in jsScriptPaths)
                    loaded.Add(new JSScript(scriptPath, jsScriptEngine));
            }

            // Load all Python scripts
            foreach (var scriptPath in Directory.GetFiles(project.Path).Where(p => Path.GetExtension(p) == ".py"))
                loaded.Add(new PythonScript(scriptPath));
        }

        public async Task<T> GenerateAsync(CancellationToken token = default)
        {
            if (IsDisposed)
                throw new InvalidOperationException("Cannot generate an already disposed runner.");

            var context = CreateContext();

            PreGenerate(context);

            if (PythonHelper.HAS_PYTHON && loaded.Any(s => s is PythonScript))
                PythonEngine.Initialize();

            await Task.WhenAll(loaded.Select(s => s.GenerateAsync(token)));

            if (PythonHelper.HAS_PYTHON && loaded.Any(s => s is PythonScript))
                PythonEngine.Shutdown();

            var groups = loaded
                .SelectMany(s => s.Groups)
                .GroupBy(g => g.Name, g => g.Elements, (key, g) => new ScriptElementGroup(key) { Elements = g.SelectMany(a => a) });

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

        public T Generate() => GenerateAsync().Result;

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

        /// <summary>
        /// Disposes resources used by this runner.
        /// </summary>
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
