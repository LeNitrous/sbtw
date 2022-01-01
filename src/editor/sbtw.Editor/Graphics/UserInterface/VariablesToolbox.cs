// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osuTK;
using sbtw.Editor.Projects;
using sbtw.Editor.Scripts;

namespace sbtw.Editor.Graphics.UserInterface
{
    public class VariablesToolbox : EditorTabbedToolbox<string>
    {
        [Cached]
        private readonly BindableDictionary<string, IEnumerable<ScriptVariableInfo>> variableMap = new BindableDictionary<string, IEnumerable<ScriptVariableInfo>>();
        private readonly VariablesTab variablesTab;
        private string currentTarget;

        public VariablesToolbox()
            : base("Variables")
        {
            AddTab(@"Scripts", new ScriptListTab(handleScriptSelection));
            Add(variablesTab = new VariablesTab());
        }

        [BackgroundDependencyLoader]
        private void load(Bindable<IProject> project)
        {
            project.BindValueChanged(e =>
            {
                variableMap.UnbindFrom(e.OldValue.Variables);
                variableMap.BindTo(e.NewValue.Variables);
            }, true);
        }

        private void handleScriptSelection(string scriptName)
        {
            if (!string.IsNullOrEmpty(currentTarget))
                RemoveTab(currentTarget);

            AddTab(scriptName, variablesTab);
            SetTab(scriptName);

            variablesTab.SetTarget(currentTarget = scriptName);
        }

        protected override TabControl<string> CreateTabControl() => new VariablesBreadcrumb
        {
            RelativeSizeAxes = Axes.X,
            Height = 24,
        };

        private class VariablesBreadcrumb : BreadcrumbControl<string>
        {
            protected override TabItem<string> CreateTabItem(string value) => new VariablesBreadcrumbItem(value);

            private class VariablesBreadcrumbItem : BreadcrumbTabItem
            {
                public VariablesBreadcrumbItem(string value)
                    : base(value)
                {
                    Text.Font = OsuFont.GetFont(Typeface.Torus, 14f);
                }

                protected override bool OnHover(HoverEvent e)
                {
                    if (Active.Value)
                        FadeHovered();

                    return false;
                }
            }
        }

        private class VariablesTab : FillFlowContainer
        {
            [Resolved]
            private Bindable<IProject> project { get; set; }

            [Resolved(canBeNull: true)]
            private Editor editor { get; set; }

            private string scriptName;
            private BindableDictionary<string, IEnumerable<ScriptVariableInfo>> variableMap;

            [BackgroundDependencyLoader]
            private void load(BindableDictionary<string, IEnumerable<ScriptVariableInfo>> variableMap)
            {
                this.variableMap = variableMap.GetBoundCopy();
                this.variableMap.BindCollectionChanged((_, __) => updateContents(), true);

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Direction = FillDirection.Vertical;
            }

            public void SetTarget(string scriptName)
            {
                this.scriptName = scriptName;
                updateContents();
            }

            private void updateContents()
            {
                Clear();

                if (scriptName == null || !variableMap.TryGetValue(scriptName, out var variables))
                    return;

                foreach (var variable in variables)
                {
                    var type = variable.Value.GetType();

                    if (type == typeof(long) || type == typeof(int))
                    {
                        var numberBox = new SettingsNumberBox
                        {
                            ShowsDefaultIndicator = false,
                            LabelText = variable.Name,
                            Current =
                            {
                                Value = Convert.ToInt32(variable.Value),
                            },
                        };
                        numberBox.Current.ValueChanged += e => updateVariable(scriptName, variable.Name, e.NewValue);
                        Add(numberBox);
                    }
                }
            }

            private ScheduledDelegate debounce;

            private void updateVariable(string script, string name, object value)
            {
                if (!project.Value.Variables.TryGetValue(script, out var variables))
                    return;

                var variable = variables.FirstOrDefault(v => v.Name == name);
                variable.Value = value;

                debounce?.Cancel();
                debounce = Scheduler.AddDelayed(() => editor?.GeneratePreview(), 500);
            }
        }

        private class ScriptListTab : FillFlowContainer
        {
            private readonly Action<string> onClick;

            public ScriptListTab(Action<string> onClick)
            {
                this.onClick = onClick;
            }

            [BackgroundDependencyLoader]
            private void load(BindableDictionary<string, IEnumerable<ScriptVariableInfo>> variableMap)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Direction = FillDirection.Vertical;
                variableMap.BindCollectionChanged((_, args) =>
                {
                    Children = variableMap.Select(p => new ScriptListItem(p.Key, onClick)).ToList();
                }, true);
            }
        }

        private class ScriptListItem : CompositeDrawable
        {
            private readonly Action<string> onClick;
            private readonly string name;

            public ScriptListItem(string name, Action<string> onClick)
            {
                this.name = name;
                this.onClick = onClick;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                RelativeSizeAxes = Axes.X;
                Height = 40;
                InternalChildren = new Drawable[]
                {
                    new ScriptItemLabel(name, onClick)
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                    new IconButton
                    {
                        Size = new Vector2(30),
                        Icon = FontAwesome.Solid.Sync,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                    },
                };
            }

            private class ScriptItemLabel : CompositeDrawable
            {
                private Colour4 hoverColour;
                private OsuSpriteText text;
                private readonly string name;
                private readonly Action<string> onClick;

                public ScriptItemLabel(string name, Action<string> onClick)
                {
                    this.name = name;
                    this.onClick = onClick;
                }

                [BackgroundDependencyLoader]
                private void load(OsuColour colours)
                {
                    hoverColour = colours.Yellow;
                    RelativeSizeAxes = Axes.Both;
                    InternalChildren = new Drawable[]
                    {
                        text = new OsuSpriteText
                        {
                            Text = name,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        new HoverClickSounds(HoverSampleSet.Submit),
                    };
                }

                protected override bool OnClick(ClickEvent e)
                {
                    onClick?.Invoke(name);
                    return true;
                }

                protected override bool OnHover(HoverEvent e)
                {
                    text.FadeColour(hoverColour, 500, Easing.OutQuint);
                    return base.OnHover(e);
                }

                protected override void OnHoverLost(HoverLostEvent e)
                {
                    text.FadeColour(Colour4.White, 500, Easing.OutQuint);
                    base.OnHoverLost(e);
                }
            }
        }
    }
}
