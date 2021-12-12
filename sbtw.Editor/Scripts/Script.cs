// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    public class Script
    {
        public readonly string Path;

        private readonly List<ScriptElementGroup> groups = new List<ScriptElementGroup>();

        public ScriptElementGroup GetGroup(string name)
        {
            var group = groups.FirstOrDefault(g => g.Name == name);

            if (group != null)
                return group;

            group = new ScriptElementGroup(this, name);
            groups.Add(group);

            return group;
        }

        public void SetVideo(string path, int offset)
        {
            if (groups.Any(g => g.Name == "Video"))
                throw new InvalidOperationException("Cannot create another video in the same script.");

            GetGroup("Video").CreateVideo(path, offset);
        }

        public IEnumerable<ScriptElementGroup> Generate()
        {
            return groups;
        }

        public Task<IEnumerable<ScriptElementGroup>> GenerateAsync(CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            return Task.Run(Generate);
        }
    }
}
