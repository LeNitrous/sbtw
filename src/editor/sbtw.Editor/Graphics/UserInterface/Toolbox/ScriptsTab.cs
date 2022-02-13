// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osuTK;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public class ScriptsTab : EditorTabbedToolboxTab
    {
        private IBindableList<ScriptExecutionResult> results;
        protected override Container<Drawable> Content { get; }

        public ScriptsTab()
        {
            InternalChild = new OsuScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = true,
                Child = Content = new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(BindableList<ScriptExecutionResult> results)
        {
            this.results = results.GetBoundCopy();
            this.results.BindCollectionChanged((_, __) => Schedule(() => Children = results.Select(s => new ScriptItem(s)).ToArray()), true);
        }

        private class ScriptItem : CompositeDrawable
        {
            private readonly ScriptExecutionResult result;

            public ScriptItem(ScriptExecutionResult result)
            {
                this.result = result;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                RelativeSizeAxes = Axes.X;
                Height = 40;
                InternalChildren = new Drawable[]
                {
                    new LabelSpriteText(result.Script)
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding { Left = 30 },
                    },
                    new BugIndicator
                    {
                        Icon = FontAwesome.Solid.Bug,
                        Size = new Vector2(14),
                        Alpha = result.Faulted ? 1 : 0,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                    new IconButton
                    {
                        Icon = FontAwesome.Regular.Edit,
                        Size = new Vector2(40),
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        TooltipText = @"Reveal script in code.",
                    }
                };
            }

            private class BugIndicator : SpriteIcon, IHasTooltip
            {
                public LocalisableString TooltipText => @"There were errors when running this script.";
            }

            private class LabelSpriteText : OsuSpriteText, IHasTooltip
            {
                public LocalisableString TooltipText { get; }

                public LabelSpriteText(IScript script)
                {
                    if (script is INamedScript named)
                        Text = named.Name;

                    if (script is FileBasedScript file)
                        TooltipText = file.Path;
                }
            }
        }
    }
}
