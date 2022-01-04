// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using osu.Framework.Bindables;

namespace sbtw.Editor.Projects
{
    public class ElementGroupSetting
    {
        public string Name { get; set; }

        public Bindable<bool> Visible { get; } = new Bindable<bool>(true);

        public Bindable<bool> ExportToDifficulty { get; } = new Bindable<bool>();
    }
}
