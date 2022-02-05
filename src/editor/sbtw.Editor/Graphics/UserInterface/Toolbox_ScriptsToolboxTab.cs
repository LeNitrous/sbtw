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
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;
using sbtw.Editor.Studios;

namespace sbtw.Editor.Graphics.UserInterface
{
    public partial class Toolbox
    {
        private class ScriptsToolboxTab : FillFlowContainer
        {
            private readonly BindableList<ScriptGenerationResult> scripts = new BindableList<ScriptGenerationResult>();
            private Bindable<IProject> project;

            protected override Container<Drawable> Content { get; }

            public ScriptsToolboxTab()
            {
                RelativeSizeAxes = Axes.Both;
                InternalChild = new OsuScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = Content = new FillFlowContainer
                    {
                        Direction = FillDirection.Vertical,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(Bindable<IProject> project)
            {
                this.project = project.GetBoundCopy();
                this.project.BindValueChanged(e =>
                {
                    scripts.UnbindBindings();

                    if (e.NewValue is Project project)
                        scripts.BindTo(project.Scripts);
                }, true);

                scripts.BindCollectionChanged((_, args) => Schedule(() => Children = scripts.Select(s => new ScriptListItem(s)).ToList()), true);
            }

            private class ScriptListItem : CompositeDrawable
            {
                private readonly ScriptGenerationResult script;

                public ScriptListItem(ScriptGenerationResult script)
                {
                    this.script = script;
                }

                [BackgroundDependencyLoader]
                private void load(Bindable<Studio> studio)
                {
                    RelativeSizeAxes = Axes.X;
                    Height = 40;
                    InternalChildren = new Drawable[]
                    {
                        new LabelSpriteText(script)
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Margin = new MarginPadding { Left = 30 },
                        },
                        new BugIndicator
                        {
                            Icon = FontAwesome.Solid.Bug,
                            Size = new Vector2(14),
                            Alpha = script.Exception != null ? 1 : 0,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        new IconButton
                        {
                            Icon = FontAwesome.Regular.Edit,
                            Size = new Vector2(40),
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Action = () => studio.Value?.Open(script.Path),
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

                    public LabelSpriteText(ScriptGenerationResult script)
                    {
                        Text = script.Name;
                        TooltipText = script.Path;
                    }
                }
            }
        }
    }
}