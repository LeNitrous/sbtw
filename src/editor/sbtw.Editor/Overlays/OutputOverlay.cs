// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Chat;
using osu.Game.Overlays;
using osuTK;
using sbtw.Editor.Overlays.Output;

namespace sbtw.Editor.Overlays
{
    public class OutputOverlay : OverlayContainer
    {
        private OutputFlowContainer flow;
        private Container outputContainer;
        private Container header;
        private Sample popInSample;
        private Sample popOutSample;

        [Cached]
        private readonly OverlayColourProvider colours = new OverlayColourProvider(OverlayColourScheme.Purple);

        public OutputOverlay()
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Both;
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            Children = new Drawable[]
            {
                outputContainer = new Container
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Height = 0.4f,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colours.Background4,
                        },
                        flow = new OutputFlowContainer
                        {
                            Name = @"content",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = 40 },
                        },
                        header = new Header
                        {
                            Name = @"header",
                            RelativeSizeAxes = Axes.X,
                            Height = 40,
                            Masking = true,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = colours.Background5,
                                },
                                new OsuSpriteText
                                {
                                    Text = @"output",
                                    Font = OsuFont.Inter.With(weight: FontWeight.Bold),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Margin = new MarginPadding { Left = 24 },
                                },
                                new FillFlowContainer
                                {
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Direction = FillDirection.Horizontal,
                                    AutoSizeAxes = Axes.Both,
                                    Spacing = new Vector2(8, 0),
                                    Margin = new MarginPadding { Right = 12 },
                                    Children = new Drawable[]
                                    {
                                        new IconButton
                                        {
                                            Icon = FontAwesome.Regular.TrashAlt,
                                            IconScale = new Vector2(0.8f),
                                            Action = () => Clear(),
                                        },
                                        new IconButton
                                        {
                                            Icon = FontAwesome.Solid.Times,
                                            IconScale = new Vector2(0.8f),
                                            Action = () => Hide(),
                                        },
                                    }
                                }
                            }
                        }
                    }
                },
            };

            popInSample = audio.Samples.Get("UI/overlay-pop-in");
            popOutSample = audio.Samples.Get("UI/overlay-pop-out");
        }

        public new void Clear()
        {
            flow.Clear();
        }

        public void AddSeparator() => flow.AddSeparator();

        public void AddLine(string message, LogLevel level = LogLevel.Verbose)
        {
            var nextMessage = new Message { Content = message, Timestamp = DateTimeOffset.Now.ToLocalTime() };

            switch (level)
            {
                case LogLevel.Verbose:
                case LogLevel.Important:
                    nextMessage.Sender = OutputUser.USER_VERBOSE;
                    Logger.Log(message, level: level);
                    break;

                case LogLevel.Error:
                    nextMessage.Sender = OutputUser.USER_ERROR;
                    break;

                case LogLevel.Debug:
                    nextMessage.Sender = OutputUser.USER_DEBUG;
                    Logger.Log(message, level: level);
                    break;
            }

            flow.AddLine(nextMessage);
        }

        public void Error(Exception e, string message)
        {
            AddLine($"{message}\n{e}", LogLevel.Error);
            Logger.Error(e, message);
        }

        public void Debug(string message) => AddLine(message, LogLevel.Debug);

        public override bool Contains(Vector2 screenSpacePos) => outputContainer.ReceivePositionalInputAt(screenSpacePos);

        private float startDragHeight;
        private bool isDragging;

        protected override bool OnDragStart(DragStartEvent e)
        {
            isDragging = header.IsHovered;

            if (!isDragging)
                return base.OnDragStart(e);

            startDragHeight = outputContainer.Height;

            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            if (!isDragging)
                return;

            float targetHeight = startDragHeight - (e.MousePosition.Y - e.MouseDownPosition.Y) / Parent.DrawSize.Y;
            outputContainer.Height = Math.Min(1, Math.Max(0.1f, targetHeight));
        }

        protected override void UpdateState(ValueChangedEvent<Visibility> state)
        {
            bool changed = state.NewValue != state.OldValue;

            switch (state.NewValue)
            {
                case Visibility.Visible:
                    if (changed)
                        popInSample?.Play();
                    break;

                case Visibility.Hidden:
                    if (changed)
                        popOutSample?.Play();
                    break;
            }

            base.UpdateState(state);
        }

        protected override void PopIn()
        {
            this.MoveToY(0, 500, Easing.OutQuint);
            this.FadeIn(500, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            this.MoveToY(Height, 500, Easing.InSine);
            this.FadeOut(500, Easing.InSine);
        }

        private class Header : Container
        {
            public override bool HandlePositionalInput => true;
        }
    }
}
