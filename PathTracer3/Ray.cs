using System.Diagnostics.CodeAnalysis;

namespace PathTracer3
{
    public struct Ray
    {
        public Ray(Vector3 origin, Vector3 direction, double tmin = 0.0, double tmax = double.PositiveInfinity, int depth = 0)
        {
            Origin = origin;
            Direction = direction;
            Tmin = tmin;
            Tmax = tmax;
            Depth = depth;
        }

        public Vector3 Origin    { get; }
        public Vector3 Direction { get; }
        public int Depth         { get; }
        public double Tmin       { get; }
        public double Tmax       { get; set; }

        public readonly Vector3 Eval(double t) => Origin + Direction * t;

        [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
        public override readonly string ToString() => $"Ray(O: {Origin}, D: {Direction})";
    }
}