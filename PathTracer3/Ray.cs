﻿using System.Diagnostics.CodeAnalysis;

namespace PathTracer3
{
    public class Ray
    {
        public Ray(Vector3 origin, Vector3 direction, double tmin = 0.0, double tmax = double.PositiveInfinity, int depth = 0)
        {
            this.Origin = origin;
            this.Direction = direction;
            this.Tmin = tmin;
            this.Tmax = tmax;
            this.Depth = depth;
        }

        public Vector3 Origin { get; }
        public Vector3 Direction { get; }
        public double Tmin { get; }
        public double Tmax { get; set; }
        public int Depth { get; }

        public Vector3 Eval(double t) => Origin + Direction * t;

        [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
        public override string ToString() => $"Ray(O: {Origin}, D: {Direction})";
    }
}