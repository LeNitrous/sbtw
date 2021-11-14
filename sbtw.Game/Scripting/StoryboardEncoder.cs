// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Text;
using sbtw.Common.Scripting;
using sbtw.Game.Projects;

namespace sbtw.Game.Scripting
{
    public class StoryboardEncoder : ScriptAssemblyRunner<StringBuilder>
    {
        public StoryboardEncoder(Project project)
            : base(project)
        {
        }

        protected override StringBuilder CreateContext()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleAnimation(StringBuilder context, ScriptedStoryboardAnimation animation)
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleSample(StringBuilder context, ScriptedStoryboardSample sample)
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleSprite(StringBuilder context, ScriptedStoryboardSprite sprite)
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleVideo(StringBuilder context, ScriptedStoryboardVideo video)
        {
            throw new System.NotImplementedException();
        }
    }
}
