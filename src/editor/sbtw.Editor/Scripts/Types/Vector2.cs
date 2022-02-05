// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;

namespace sbtw.Editor.Scripts.Types
{
    public struct Vector2 : IEquatable<Vector2>
    {
        private readonly osuTK.Vector2 underlying;

        public double X => underlying.X;
        public double Y => underlying.Y;

        public Vector2(double x, double y)
        {
            underlying = new osuTK.Vector2((float)x, (float)y);
        }

        public Vector2(double value)
            : this(value, value)
        {
        }

        private Vector2(osuTK.Vector2 vector)
        {
            underlying = vector;
        }

        public void Normalize() => underlying.Normalize();
        public void NormalizeFast() => underlying.NormalizeFast();
        public Vector2 Normalized() => underlying.Normalized();

        public static implicit operator osuTK.Vector2(Vector2 v) => v.underlying;
        public static implicit operator Vector2(osuTK.Vector2 v) => new Vector2(v);
        public static Vector2 Add(Vector2 a, Vector2 b) => osuTK.Vector2.Add(a, b);
        public static Vector2 BaryCentric(Vector2 vec, Vector2 min, Vector2 max, double u, double v) => osuTK.Vector2.BaryCentric(vec, min, max, (float)u, (float)v);
        public static Vector2 ComponentMax(Vector2 a, Vector2 b) => osuTK.Vector2.ComponentMax(a, b);
        public static Vector2 ComponentMin(Vector2 a, Vector2 b) => osuTK.Vector2.ComponentMin(a, b);
        public static double Distance(Vector2 a, Vector2 b) => osuTK.Vector2.Distance(a, b);
        public static double DistanceSquared(Vector2 a, Vector2 b) => osuTK.Vector2.DistanceSquared(a, b);
        public static Vector2 Divide(Vector2 a, Vector2 b) => osuTK.Vector2.Divide(a, b);
        public static Vector2 Divide(Vector2 a, double b) => osuTK.Vector2.Divide(a, (float)b);
        public static double Dot(Vector2 a, Vector2 b) => osuTK.Vector2.Dot(a, b);
        public static Vector2 Lerp(Vector2 a, Vector2 b, double blend) => osuTK.Vector2.Lerp(a, b, (float)blend);
        public static Vector2 MagnitudeMax(Vector2 a, Vector2 b) => osuTK.Vector2.MagnitudeMax(a, b);
        public static Vector2 MagnitudeMin(Vector2 a, Vector2 b) => osuTK.Vector2.MagnitudeMin(a, b);
        public static Vector2 Multiply(Vector2 a, Vector2 b) => osuTK.Vector2.Multiply(a, b);
        public static Vector2 Multiply(Vector2 a, double b) => osuTK.Vector2.Multiply(a, (float)b);
        public static Vector2 Normalize(Vector2 a) => osuTK.Vector2.Normalize(a);
        public static Vector2 NormalizeFast(Vector2 a) => osuTK.Vector2.NormalizeFast(a);
        public static double PerpDot(Vector2 a, Vector2 b) => osuTK.Vector2.PerpDot(a, b);
        public static Vector2 Subtract(Vector2 a, Vector2 b) => osuTK.Vector2.Subtract(a, b);

        public static Vector2 operator +(Vector2 left, Vector2 right) => left.underlying + right.underlying;
        public static Vector2 operator -(Vector2 vec) => -vec.underlying;
        public static Vector2 operator -(Vector2 left, Vector2 right) => left.underlying - right.underlying;
        public static Vector2 operator *(Vector2 left, Vector2 right) => left.underlying * right.underlying;
        public static Vector2 operator *(Vector2 left, double scale) => left.underlying * (float)scale;
        public static Vector2 operator *(double scale, Vector2 right) => (float)scale * right.underlying;
        public static Vector2 operator /(Vector2 left, double scale) => left.underlying / (float)scale;
        public static bool operator ==(Vector2 left, Vector2 right) => left.underlying == right.underlying;
        public static bool operator !=(Vector2 left, Vector2 right) => left.underlying != right.underlying;

        public bool Equals(Vector2 other) => other == this;

        public override bool Equals(object obj) => obj is Vector2 vector && underlying.Equals(vector.underlying);

        public override int GetHashCode() => HashCode.Combine(underlying, X, Y);
    }
}
