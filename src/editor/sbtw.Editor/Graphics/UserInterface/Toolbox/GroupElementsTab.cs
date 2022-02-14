// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osuTK;
using sbtw.Editor.Beatmaps;
using sbtw.Editor.Configuration;
using sbtw.Editor.Scripts;
using sbtw.Editor.Scripts.Elements;

namespace sbtw.Editor.Graphics.UserInterface.Toolbox
{
    public class GroupElementsTab : EditorTabbedToolboxTab
    {
        protected override Container<Drawable> Content { get; }

        private IBindable<Group> selected;

        public GroupElementsTab()
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
        private void load(EditorSessionStatics statics)
        {
            selected = statics.GetBindable<Group>(EditorSessionStatic.GroupSelected);
            Children = selected.Value.Elements.Select<IScriptElement, Drawable>(e =>
            {
                switch (e)
                {
                    case ScriptedAnimation animation:
                        return new GroupAnimation(animation);

                    case ScriptedSprite sprite:
                        return new GroupSprite(sprite);

                    case ScriptedVideo video:
                        return new GroupVideo(video);

                    case ScriptedSample sample:
                        return new GroupSample(sample);

                    default:
                        return null;
                }
            }).ToArray();
        }

        private abstract class GroupElementItem<T> : CompositeDrawable, IHasCustomTooltip<T>
            where T : IScriptElement
        {
            protected abstract IconUsage Icon { get; }
            public T TooltipContent { get; }
            private readonly SpriteIcon indicator;

            public GroupElementItem(T element)
            {
                TooltipContent = element;
                RelativeSizeAxes = Axes.X;
                Height = 30;
                InternalChild = new GridContainer
                {
                    AutoSizeAxes = Axes.Both,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 40),
                        new Dimension(GridSizeMode.Distributed),
                        new Dimension(GridSizeMode.Absolute, 40),
                    },
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 30),
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new SpriteIcon
                                {
                                    Icon = Icon,
                                    Size = new Vector2(20),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                },
                            },
                            new OsuSpriteText
                            {
                                Text = element.Path,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = indicator = new MissingAssetIndicator
                                {
                                    Icon = FontAwesome.Solid.ExclamationTriangle,
                                    Size = new Vector2(20),
                                    Alpha = 0,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                },
                            }
                        }
                    },
                };
            }

            [BackgroundDependencyLoader]
            private void load(IBeatmapProvider beatmapProvider)
            {
                string path = (beatmapProvider.BeatmapSet as BeatmapSetInfo).GetPathForFile(TooltipContent.Path);
                indicator.Alpha = string.IsNullOrEmpty(path) ? 1 : 0;
            }


            public abstract ITooltip<T> GetCustomTooltip();

            private class MissingAssetIndicator : SpriteIcon, IHasTooltip
            {
                public LocalisableString TooltipText => @"The resource for this asset could not be found.";
            }

            protected abstract class GroupElementTooltip : VisibilityContainer, ITooltip<T>
            {
                protected override Container<Drawable> Content { get; }

                private readonly Box background;

                public GroupElementTooltip()
                {
                    AutoSizeAxes = Axes.Both;
                    Masking = true;
                    CornerRadius = 5;

                    InternalChildren = new Drawable[]
                    {
                        background = new Box
                        {
                            RelativeSizeAxes = Axes.Both
                        },
                        Content = new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            AutoSizeDuration = 200,
                            AutoSizeEasing = Easing.OutQuint,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding(10),
                            Margin = new MarginPadding { Right = 10 },
                            Spacing = new Vector2(0, 5),
                        }
                    };
                }

                [BackgroundDependencyLoader]
                private void load(OsuColour colours)
                {
                    background.Colour = colours.Gray3;
                }

                private T last;

                public void SetContent(T content)
                {
                    if (last?.Equals(content) ?? false)
                        return;

                    last = content;

                    Clear();

                    SetContent(content, true);
                }

                protected virtual void SetContent(T content, bool _)
                {
                    Add(new TextFlowContainer(t => t.Font = t.Font.With(size: 16))
                    {
                        AutoSizeAxes = Axes.Both,
                        Text = string.Join('\n', GetTextContent(content)),
                    });
                }

                public void Move(Vector2 pos) => Position = pos;
                protected override void PopIn() => this.FadeIn(200, Easing.OutQuint);
                protected override void PopOut() => this.FadeOut(200, Easing.OutQuint);
                protected virtual IEnumerable<string> GetTextContent(T content) => new[]
                {
                    $"Type: {content.GetType().Name.Replace("Scripted", string.Empty)}",
                    $"Layer: {Enum.GetName(content.Layer)}",
                    $"Start Time: {content.StartTime:0}",
                };
            }
        }

        private class GroupSprite : GroupElementItem<ScriptedSprite>
        {
            public GroupSprite(ScriptedSprite element)
                : base(element)
            {
            }

            protected override IconUsage Icon => FontAwesome.Regular.FileImage;
            public override ITooltip<ScriptedSprite> GetCustomTooltip() => new GroupSpriteItemTooltip();

            protected class GroupSpriteItemTooltip : GroupElementTooltip
            {
                protected override IEnumerable<string> GetTextContent(ScriptedSprite content)
                    => base.GetTextContent(content).Append($"End Time: {content.EndTime:0}");
            }
        }

        private class GroupVideo : GroupElementItem<ScriptedVideo>
        {
            public GroupVideo(ScriptedVideo element)
                : base(element)
            {
            }

            protected override IconUsage Icon => FontAwesome.Regular.FileVideo;
            public override ITooltip<ScriptedVideo> GetCustomTooltip()
                => new GroupVideoItemTooltip();

            private class GroupVideoItemTooltip : GroupElementTooltip
            {
                protected override IEnumerable<string> GetTextContent(ScriptedVideo content)
                    => base.GetTextContent(content).SkipLast(1).Append($"Offset: {content.StartTime}");
            }
        }

        private class GroupAnimation : GroupSprite
        {
            public GroupAnimation(ScriptedAnimation element)
                : base(element)
            {
            }

            protected override IconUsage Icon => FontAwesome.Regular.Images;
            public override ITooltip<ScriptedSprite> GetCustomTooltip()
                => new GroupAnimationItemTooltip();

            private class GroupAnimationItemTooltip : GroupSpriteItemTooltip
            {
                protected override IEnumerable<string> GetTextContent(ScriptedSprite content)
                {
                    var anim = content as ScriptedAnimation;
                    return base.GetTextContent(content).Concat(new[]
                    {
                        $"Loop Type: {Enum.GetName(anim.LoopType)}",
                        $"Frame Count: {anim.FrameCount}",
                        $"Frame Delay: ${anim.FrameDelay}",
                    });
                }
            }
        }

        private class GroupSample : GroupElementItem<ScriptedSample>
        {
            public GroupSample(ScriptedSample element)
                : base(element)
            {
            }

            protected override IconUsage Icon => FontAwesome.Regular.FileAudio;
            public override ITooltip<ScriptedSample> GetCustomTooltip()
            {
                throw new NotImplementedException();
            }

            private class GroupSampleItemTooltip : GroupElementTooltip
            {
                protected override IEnumerable<string> GetTextContent(ScriptedSample content)
                    => base.GetTextContent(content).Append($"Volume: {content.Volume}%");
            }
        }
    }
}
