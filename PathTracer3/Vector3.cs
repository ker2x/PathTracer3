﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace PathTracer3
{
    public struct Vector3 : IEquatable<Vector3>
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3(double x, double y, double z) => (X, Y, Z) = (x, y, z);
        public Vector3(double v = 0.0d) : this(v, v, v) {}
        public void Zero() => (X, Y, Z) = (0.0, 0.0, 0.0);
        
        public override string ToString() => $"Vector3({X}, {Y}, {Z})";
        public override int GetHashCode() => X.GetHashCode() ^  Y.GetHashCode() ^  Z.GetHashCode();
        public bool HasNaNs()             => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z);


        //Math operators
        public static Vector3 operator -(Vector3 v) => new(-v.X, -v.Y, -v.Z);
        public static Vector3 operator -(Vector3 v, double a) => new(v.X - a, v.Y - a, v.Z - a);
        public static Vector3 operator -(double a, Vector3 v) => new(a - v.X, a - v.Y, a - v.Z);
        public static Vector3 operator -(Vector3 v1, Vector3 v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

        public static Vector3 operator +(Vector3 v, double a) => new(v.X + a, v.Y + a, v.Z + a);
        public static Vector3 operator +(double a, Vector3 v) => new(a + v.X, a + v.Y, a + v.Z);
        public static Vector3 operator +(Vector3 v1, Vector3 v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

        public static Vector3 operator *(Vector3 v1, Vector3 v2) => new(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        public static Vector3 operator *(Vector3 v, double a) => new(v.X * a, v.Y * a, v.Z * a);
        public static Vector3 operator *(double a, Vector3 v) => new(a * v.X, a * v.Y, a * v.Z);

        public static Vector3 operator /(Vector3 v1, Vector3 v2) => new(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        public static Vector3 operator /(Vector3 v, double a) => new(v.X / a, v.Y / a, v.Z / a);
        public static Vector3 operator /(double a, Vector3 v) => new(a / v.X, a / v.Y, a / v.Z);
        
        
        //Math comparators
        public override bool Equals(object obj) => obj != null && Equals((Vector3) obj);
        
        public bool Equals(Vector3 v) => 
            Math.Abs(X - v.X) <= double.Epsilon
            && Math.Abs(Y - v.Y) <= double.Epsilon
            && Math.Abs(Z - v.Z) <= double.Epsilon;

        public static bool operator ==(Vector3 v1, Vector3 v2) => 
            Math.Abs(v1.X - v2.X) <= double.Epsilon
            && Math.Abs(v1.Y - v2.Y) <= double.Epsilon
            && Math.Abs(v1.Z - v2.Z) <= double.Epsilon;

        public static bool operator !=(Vector3 v1, Vector3 v2) => 
            Math.Abs(v1.X - v2.X) > double.Epsilon
            || Math.Abs(v1.Y - v2.Y) > double.Epsilon
            || Math.Abs(v1.Z - v2.Z) > double.Epsilon;

        public static bool operator <(Vector3 v1, Vector3 v2) => v1.X < v2.X && v1.Y < v2.Y && v1.Z < v2.Z;
        public static bool operator >(Vector3 v1, Vector3 v2) => v1.X > v2.X && v1.Y > v2.Y && v1.Z > v2.Z;

        public static bool operator <=(Vector3 v1, Vector3 v2) => v1.X <= v2.X && v1.Y <= v2.Y && v1.Z <= v2.Z;
        public static bool operator >=(Vector3 v1, Vector3 v2) => v1.X >= v2.X && v1.Y >= v2.Y && v1.Z >= v2.Z;


        // Math methods
        public double Dot(Vector3 v) => X * v.X + Y * v.Y + Z * v.Z;
        public Vector3 Cross(Vector3 v) => new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);

        
        public static Vector3 Abs(Vector3 v) => new(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
        public static Vector3 Sqrt(Vector3 v) => new(Math.Sqrt(v.X), Math.Sqrt(v.Y), Math.Sqrt(v.Z));
        public static Vector3 Pow(Vector3 v, double a) => new(Math.Pow(v.X, a), Math.Pow(v.Y, a), Math.Pow(v.Z, a));

        public static Vector3 Min(Vector3 v1, Vector3 v2) => new(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y), Math.Min(v1.Z, v2.Z));
        public static Vector3 Max(Vector3 v1, Vector3 v2) => new(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y), Math.Max(v1.Z, v2.Z));

        public static Vector3 Round(Vector3 v1) => new(Math.Round(v1.X), Math.Round(v1.Y), Math.Round(v1.Z));
        public static Vector3 Floor(Vector3 v1) => new(Math.Floor(v1.X), Math.Floor(v1.Y), Math.Floor(v1.Z));
        public static Vector3 Ceil(Vector3 v1)  => new(Math.Ceiling(v1.X), Math.Ceiling(v1.Y), Math.Ceiling(v1.Z));
        public static Vector3 Trunc(Vector3 v1) => new(Math.Truncate(v1.X), Math.Truncate(v1.Y), Math.Truncate(v1.Z));

        public static Vector3 Clamp(Vector3 v, double low = 0.0, double high = 1.0) =>
            new(MathUtils.Clamp(v.X, low, high),
                MathUtils.Clamp(v.Y, low, high),
                MathUtils.Clamp(v.Z, low, high));

        public static Vector3 Lerp(double a, Vector3 v1, Vector3 v2) => v1 + a * (v2 - v1);

        public int MinDimension() => X < Y && X < Z ? 0 : Y < Z ? 1 : 2;
        public int MaxDimension() => X > Y && X > Z ? 0 : Y > Z ? 1 : 2;
        
        public double Min() => X < Y && X < Z ? X : Y < Z ? Y : Z;
        public double Max() => X > Y && X > Z ? X : Y > Z ? Y : Z;

        public double Norm2_squared() => X * X + Y * Y + Z * Z;
        private double Norm2() => Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector3 Normalize()
        {
            var a = 1.0 / Norm2();
            X *= a;
            Y *= a;
            Z *= a;
            return this;
        }
    }
}