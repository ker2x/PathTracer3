using System;
using System.Diagnostics.CodeAnalysis;

namespace PathTracer3
{
    public static class Sampling
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static Vector3 UniformSampleOnHemisphere(double u1, double u2)
        {
            var sinTheta = Math.Sqrt(Math.Max(0.0, 1.0 - u1 * u1));
            var phi = 2.0 * Math.PI * u2;
            return new Vector3(Math.Cos(phi) * sinTheta, Math.Sin(phi) * sinTheta, u1);
        }

        public static Vector3 CosineWeightedSampleOnHemisphere(double u1, double u2)
        {
            var cosTheta = Math.Sqrt(1.0 - u1);
            var sinTheta = Math.Sqrt(u1);
            var phi = 2.0 * Math.PI * u2;
            return new Vector3(Math.Cos(phi) * sinTheta, Math.Sin(phi) * sinTheta, cosTheta);
        }
    }
}