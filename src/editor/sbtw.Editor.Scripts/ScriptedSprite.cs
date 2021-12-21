// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Storyboards;
using osuTK;

namespace sbtw.Editor.Scripts
{
    public class ScriptedSprite : IScriptedElementWithDuration
    {

        public string Path { get; }

        public Script Owner { get; }

        public Layer Layer { get; }

        public Anchor Origin { get; }

        public Vector2 InitialPosition { get; }

        public double StartTime { get; }

        public double EndTime { get; }

        internal readonly List<ScriptedCommandLoop> Loops = new List<ScriptedCommandLoop>();

        internal readonly List<ScriptedCommandTrigger> Triggers = new List<ScriptedCommandTrigger>();

        internal readonly ScriptedCommandTimelineGroup Timeline = new ScriptedCommandTimelineGroup();

        private CommandTimelineGroup context;

        private CommandTimelineGroup currentContext
        {
            get => context ?? Timeline;
            set => context = value;
        }

        public ScriptedSprite(Script owner, Layer layer, string path, Anchor origin, Vector2 initialPosition)
        {
            Owner = owner;
            Layer = layer;
            Path = path;
            Origin = origin;
            InitialPosition = initialPosition;
        }

        public void Move(Easing easing, double startTime, double endTime, Vector2 startPosition, Vector2 endPosition)
            => (currentContext as IScriptedCommandTimelineGroup)?.Move.Add(easing, startTime, endTime, startPosition, endPosition);

        public void Move(Easing easing, double startTime, double endTime, Vector2 startPosition, float endX, float endY)
            => Move(easing, startTime, endTime, startPosition, new Vector2(endX, endY));

        public void Move(Easing easing, double startTime, double endTime, float startX, float startY, Vector2 endPosition)
            => Move(easing, startTime, endTime, new Vector2(startX, startY), endPosition);

        public void Move(Easing easing, double startTime, double endTime, float startX, float startY, float endX, float endY)
            => Move(easing, startTime, endTime, new Vector2(startX, startY), new Vector2(endX, endY));

        public void Move(double startTime, double endTime, Vector2 startPosition, Vector2 endPosition)
            => Move(Easing.None, startTime, endTime, startPosition, endPosition);

        public void Move(double startTime, double endTime, Vector2 startPosition, float endX, float endY)
            => Move(Easing.None, startTime, endTime, startPosition, new Vector2(endX, endY));

        public void Move(double startTime, double endTime, float startX, float startY, Vector2 endPosition)
            => Move(Easing.None, startTime, endTime, new Vector2(startX, startY), endPosition);

        public void Move(double startTime, double endTime, float startX, float startY, float endX, float endY)
            => Move(Easing.None, startTime, endTime, new Vector2(startX, startY), new Vector2(endX, endY));

        public void Move(double time, Vector2 position)
            => Move(Easing.None, time, time, position, position);

        public void Move(double time, float x, float y)
            => Move(time, new Vector2(x, y));

        public void MoveX(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.X.Add(easing, startTime, endTime, start, end);

        public void MoveX(double startTime, double endTime, float start, float end)
            => MoveX(Easing.None, startTime, endTime, start, end);

        public void MoveX(double time, float x)
            => MoveX(time, time, x, x);

        public void MoveY(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.Y.Add(easing, startTime, endTime, start, end);

        public void MoveY(double startTime, double endTime, float start, float end)
            => MoveY(Easing.None, startTime, endTime, start, end);

        public void MoveY(double time, float x)
            => MoveY(time, time, x, x);

        public void Scale(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.Scale.Add(easing, startTime, endTime, start, end);

        public void Scale(double startTime, double endTime, float start, float end)
            => Scale(Easing.None, startTime, endTime, start, end);

        public void Scale(double time, float x)
            => Scale(time, time, x, x);

        public void ScaleVec(Easing easing, double startTime, double endTime, Vector2 startScale, Vector2 endScale)
            => currentContext.VectorScale.Add(easing, startTime, endTime, startScale, endScale);

        public void ScaleVec(Easing easing, double startTime, double endTime, Vector2 startPosition, float endX, float endY)
            => ScaleVec(easing, startTime, endTime, startPosition, new Vector2(endX, endY));

        public void ScaleVec(Easing easing, double startTime, double endTime, float startX, float startY, Vector2 endPosition)
            => ScaleVec(easing, startTime, endTime, new Vector2(startX, startY), endPosition);

        public void ScaleVec(Easing easing, double startTime, double endTime, float startX, float startY, float endX, float endY)
            => ScaleVec(easing, startTime, endTime, new Vector2(startX, startY), new Vector2(endX, endY));

        public void ScaleVec(double startTime, double endTime, Vector2 startPosition, Vector2 endPosition)
            => ScaleVec(Easing.None, startTime, endTime, startPosition, endPosition);

        public void ScaleVec(double startTime, double endTime, Vector2 startPosition, float endX, float endY)
            => ScaleVec(Easing.None, startTime, endTime, startPosition, new Vector2(endX, endY));

        public void ScaleVec(double startTime, double endTime, float startX, float startY, Vector2 endPosition)
            => ScaleVec(Easing.None, startTime, endTime, new Vector2(startX, startY), endPosition);

        public void ScaleVec(double startTime, double endTime, float startX, float startY, float endX, float endY)
            => ScaleVec(Easing.None, startTime, endTime, new Vector2(startX, startY), new Vector2(endX, endY));

        public void ScaleVec(double time, Vector2 position)
            => ScaleVec(Easing.None, time, time, position, position);

        public void ScaleVec(double time, float x, float y)
            => ScaleVec(time, new Vector2(x, y));

        public void Rotate(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.Rotation.Add(easing, startTime, endTime, start, end);

        public void Rotate(double startTime, double endTime, float start, float end)
            => Rotate(Easing.None, startTime, endTime, start, end);

        public void Rotate(double time, float x)
            => Rotate(time, time, x, x);

        public void Fade(Easing easing, double startTime, double endTime, float start, float end)
            => currentContext.Alpha.Add(easing, startTime, endTime, start, end);

        public void Fade(double startTime, double endTime, float start, float end)
            => Fade(Easing.None, startTime, endTime, start, end);

        public void Fade(double time, float x)
            => Fade(time, time, x, x);

        public void Color(Easing easing, double startTime, double endTime, Colour4 startColor, Colour4 endColor)
            => currentContext.Colour.Add(easing, startTime, endTime, new Colour4(startColor.R, startColor.G, startColor.B, 1.0f), new Colour4(endColor.R, endColor.G, endColor.B, 1.0f));

        public void Color(Easing easing, double startTime, double endTime, Colour4 startColor, float endRed, float endBlue, float endGreen)
            => Color(easing, startTime, endTime, startColor, new Colour4(endRed, endBlue, endGreen, 1.0f));

        public void Color(Easing easing, double startTime, double endTime, float startRed, float startBlue, float startGreen, Colour4 endColor)
            => Color(easing, startTime, endTime, new Colour4(startRed, startBlue, startGreen, 1.0f), endColor);

        public void Color(Easing easing, double startTime, double endTime, float startRed, float startBlue, float startGreen, float endRed, float endBlue, float endGreen)
            => Color(easing, startTime, endTime, new Colour4(startRed, startBlue, startGreen, 1.0f), new Colour4(endRed, endBlue, endGreen, 1.0f));

        public void Color(double startTime, double endTime, Colour4 startColor, Colour4 endColor)
            => Color(Easing.None, startTime, endTime, startColor, endColor);

        public void Color(double startTime, double endTime, Colour4 startColor, float endRed, float endBlue, float endGreen)
            => Color(Easing.None, startTime, endTime, startColor, new Colour4(endRed, endBlue, endGreen, 1.0f));

        public void Color(double startTime, double endTime, float startRed, float startBlue, float startGreen, Colour4 endColor)
            => Color(Easing.None, startTime, endTime, new Colour4(startRed, startBlue, startGreen, 1.0f), endColor);

        public void Color(double startTime, double endTime, float startRed, float startBlue, float startGreen, float endRed, float endBlue, float endGreen)
            => Color(Easing.None, startTime, endTime, new Colour4(startRed, startBlue, startGreen, 1.0f), new Colour4(endRed, endBlue, endGreen, 1.0f));

        public void Color(double time, Colour4 color)
            => Color(Easing.None, time, time, color, color);

        public void Color(double time, float red, float blue, float green)
            => Color(time, new Colour4(red, blue, green, 1.0f));

        public void Color(Easing easing, double startTime, double endTime, string startHex, string endHex)
            => Color(easing, startTime, endTime, Colour4.FromHex(startHex), Colour4.FromHex(endHex));

        public void Color(double startTime, double endTime, string startHex, string endHex)
            => Color(Easing.None, startTime, endTime, startHex, endHex);

        public void Color(double time, string hex)
            => Color(time, time, hex, hex);

        public void ColorRGB(Easing easing, double startTime, double endTime, byte startRed, byte startBlue, byte startGreen, byte endRed, byte endBlue, byte endGreen)
            => Color(easing, startTime, endTime, new Colour4(startRed, startGreen, startBlue, 255), new Colour4(endRed, endGreen, endBlue, 255));

        public void ColorRGB(double startTime, double endTime, byte startRed, byte startBlue, byte startGreen, byte endRed, byte endBlue, byte endGreen)
            => ColorRGB(Easing.None, startTime, endTime, startRed, startGreen, startBlue, endRed, endGreen, endBlue);

        public void ColorRGB(double time, byte red, byte blue, byte green)
            => ColorRGB(time, time, red, blue, green, red, blue, green);

        public void ColorHSL(Easing easing, double startTime, double endTime, float startHue, float startSaturation, float startLightness, float endHue, float endSaturation, float endLightness)
            => Color(easing, startTime, endTime, Colour4.FromHSL(startHue, startSaturation, startLightness), Colour4.FromHSL(endHue, endSaturation, endLightness));

        public void ColorHSL(double startTime, double endTime, float startHue, float startSaturation, float startLightness, float endHue, float endSaturation, float endLightness)
            => ColorHSL(Easing.None, startTime, endTime, startHue, startSaturation, startLightness, endHue, endSaturation, endLightness);

        public void ColorHSL(double time, float hue, float saturation, float lightness)
            => ColorHSL(time, time, hue, saturation, lightness, hue, saturation, lightness);

        public void ColorHSV(Easing easing, double startTime, double endTime, float startHue, float startSaturation, float startValue, float endHue, float endSaturation, float endValue)
            => Color(easing, startTime, endTime, Colour4.FromHSV(startHue, startSaturation, startValue), Colour4.FromHSV(endHue, endSaturation, endValue));

        public void ColorHSV(double startTime, double endTime, float startHue, float startSaturation, float startValue, float endHue, float endSaturation, float endValue)
            => ColorHSV(Easing.None, startTime, endTime, startHue, startSaturation, startValue, endHue, endSaturation, endValue);

        public void ColorHSV(double time, float hue, float saturation, float value)
            => ColorHSV(time, time, hue, saturation, value, hue, saturation, value);

        public void FlipH(Easing easing, double startTime, double endTime)
            => currentContext.FlipH.Add(easing, startTime, endTime, true, startTime == endTime);

        public void FlipH(double startTime, double endTime)
            => FlipH(Easing.None, startTime, endTime);

        public void FlipH(double time)
            => FlipH(time, time);

        public void FlipV(Easing easing, double startTime, double endTime)
            => currentContext.FlipV.Add(easing, startTime, endTime, true, startTime == endTime);

        public void FlipV(double startTime, double endTime)
            => FlipV(Easing.None, startTime, endTime);

        public void FlipV(double time)
            => FlipV(Easing.None, time, time);

        public void Additive(Easing easing, double startTime, double endTime)
            => currentContext.BlendingParameters.Add(easing, startTime, endTime, BlendingParameters.Additive, startTime == endTime ? BlendingParameters.Additive : BlendingParameters.Inherit);

        public void Additive(double startTime, double endTime)
            => Additive(Easing.None, startTime, endTime);

        public void Additive(double time)
            => Additive(Easing.None, time, time);

        public void StartLoopGroup(double startTime, int repeatCount)
        {
            if (context != null)
                throw new InvalidOperationException("Cannot start a new group when an existing group is active.");

            var loop = new ScriptedCommandLoop(startTime, repeatCount - 1);
            Loops.Add(loop);
            currentContext = loop;
        }

        public void StartTriggerGroup(string triggerName, double startTime, double endTime, int group = 0)
        {
            if (context != null)
                throw new InvalidOperationException("Cannot start a new group when an existing group is active.");

            var trigger = new ScriptedCommandTrigger(triggerName, startTime, endTime, group);
            Triggers.Add(trigger);
            currentContext = trigger;
        }

        public void EndGroup()
            => currentContext = null;
    }
}
