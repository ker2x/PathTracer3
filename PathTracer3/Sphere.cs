using System;

namespace PathTracer3
{
    public class Sphere
    {
        public enum MaterialType
        {
            Diffuse,
            Specular,
            Refractive
        }

        public const double EpsilonSphere = 1e-4;

        public Sphere(double radius, Vector3 position, Vector3 emission, Vector3 color, MaterialType materialType)
        {
            Radius = radius;
            Position = position;
            Emission = emission;
            Color = color;
            Material = materialType;
        }

        private double Radius { get;  }
        public Vector3 Position { get; }
        public Vector3 Emission { get; }
        public Vector3 Color { get; }
        public MaterialType Material { get; }

        public bool Intersect(Ray ray)
        {
            var op = Position - ray.Origin;
            var dop = ray.Direction.Dot(op);
            var destination = dop * dop - op.Dot(op) + Radius * Radius;

            if (destination < 0) return false;

            var sqrtD = Math.Sqrt(destination);

            var tmin = dop - sqrtD;
            if (ray.Tmin < tmin && tmin < ray.Tmax)
            {
                ray.Tmax = tmin;
                return true;
            }

            var tmax = dop + sqrtD;
            // ReSharper disable once InvertIf
            if (ray.Tmin < tmax && tmax < ray.Tmax)
            {
                ray.Tmax = tmax;
                return true;
            }

            return false;
        }
    }
}