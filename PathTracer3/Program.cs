using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PathTracer3
{
    internal class Program
    {
		private static void Main(string[] args)
		{
			const int width = 800;
			const int height = 600;
			const int defaultSample = 16;
			const double fov = 0.5135;
			
			var startTime = DateTime.Now;
			var rng = new Rng();
			var nbSamples = (args.Length > 0) ? int.Parse(args[0]) / 4 : defaultSample;
			
			Console.WriteLine($"Starting {startTime}");

			var eye = new Vector3(50, 52, 295.6);
			var gaze = new Vector3(0, -0.042612, -1).Normalize();
			var cx = new Vector3(width * fov / height, 0.0, 0.0);
			var cy = (cx.Cross(gaze)).Normalize() * fov;
			var ls = new Vector3[width * height];
			
			for (var index = 0; index < width * height; ++index) {
				ls[index] = new Vector3();
			}

			Parallel.For(0, height, y => RenderFunc(y, width, height, nbSamples, eye, gaze, cx, cy, ls, rng));

			ImageIO.WritePPM(width, height, ls);
			Console.WriteLine($"\nRun time : {(DateTime.Now - startTime).Seconds} seconds");
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private static void RenderFunc(int y, int w, int h, int nbSamples,
			                           Vector3 eye, Vector3 gaze,
									   Vector3 cx,  Vector3 cy,
									   Vector3[] Ls, Rng rng) {

			for (var x = 0; x < w; ++x) {									// row
				for (int sy = 0, i = (h - 1 - y) * w + x; sy < 2; ++sy) {	//column
					for (var sx = 0; sx < 2; ++sx) {						//subpixel row
						var L = new Vector3();
						for (var s = 0; s < nbSamples; ++s) {				//subpixel column
							var u1 = 2.0 * rng.UniformFloat();
							var u2 = 2.0 * rng.UniformFloat();
							var dx = u1 < 1 ? Math.Sqrt(u1) - 1.0 : 1.0 - Math.Sqrt(2.0 - u1);
							var dy = u2 < 1 ? Math.Sqrt(u2) - 1.0 : 1.0 - Math.Sqrt(2.0 - u2);
							var d = cx * (((sx + 0.5 + dx) / 2 + x) / w - 0.5) +
							        cy * (((sy + 0.5 + dy) / 2 + y) / h - 0.5) + gaze;
							L += Radiance(new Ray(eye + d * 130, d.Normalize(), Sphere.EpsilonSphere), rng) * (1.0 / nbSamples);
						}
						Ls[i] += 0.25 * Vector3.Clamp(L);
					}
				}
			}

		}

		// Scene
		private const double RefractiveIndexOut = 1.0;
		private const double RefractiveIndexIn  = 1.5;

		private static readonly Sphere[] Spheres =
		{
			new(1e5,  new Vector3(1e5 + 1, 40.8, 81.6),   new Vector3(),   new Vector3(0.75,0.25,0.25),   Sphere.MaterialType.Diffuse),    //Left
			new(1e5,  new Vector3(-1e5 + 99, 40.8, 81.6), new Vector3(),   new Vector3(0.25,0.25,0.75),   Sphere.MaterialType.Diffuse),	//Right
			new(1e5,  new Vector3(50, 40.8, 1e5),         new Vector3(),   new Vector3(0.25, 0.75, 0.25), Sphere.MaterialType.Diffuse),	//Back
			new(1e5,  new Vector3(50, 40.8, -1e5 + 170),  new Vector3(),   new Vector3(),                      Sphere.MaterialType.Diffuse),	//Front
			new(1e5,  new Vector3(50, 1e5, 81.6),         new Vector3(),   new Vector3(0.75),                Sphere.MaterialType.Diffuse),    //Bottom
			new(1e5,  new Vector3(50, -1e5 + 81.6, 81.6), new Vector3(),   new Vector3(0.75),                Sphere.MaterialType.Diffuse),	//Top
			new(16.5, new Vector3(27, 16.5, 47),          new Vector3(),   new Vector3(0.499),               Sphere.MaterialType.Specular),	//Mirror
			new(16.5, new Vector3(73, 16.5, 78),          new Vector3(),   new Vector3(0.999),               Sphere.MaterialType.Refractive),	//Glass
			new(600,  new Vector3(50, 681.6 - .27, 81.6), new Vector3(12), new Vector3(),                    Sphere.MaterialType.Diffuse)		//Light
		};

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private static bool Intersect(Ray ray, out int id) {
			id = 0;
			var hit = false;
			for (var i = 0; i < Spheres.Length; ++i) {
				if (Spheres[i].Intersect(ray)) {
					hit = true;
					id = i;
				}
			}
			return hit;
		}

		public static bool Intersect(Ray ray)
		{
			for (var index = 0; index < Spheres.Length; index++)
			{
				var t = Spheres[index];
				if (t.Intersect(ray)) return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public static Vector3 Radiance(Ray ray, Rng rng) {
			var r = ray;
			var l = new Vector3();
			var f = new Vector3(1.0);

			while (true) {
				if (!Intersect(r, out var id)) {
					return l;
				}

				var shape = Spheres[id];
				var p = r.Eval(r.Tmax);
				var n = (p - shape.Position).Normalize();

				l += f * shape.Emission;
				f *= shape.Color;

				// Russian roulette
				if (r.Depth > 4) {
					var continueProbability = shape.Color.Max();
					if (rng.UniformFloat() >= continueProbability) {
						return l;
					}
					f /= continueProbability;
				}

				// Next path segment
				switch (shape.Material) {
					case Sphere.MaterialType.Specular: {
							var d = Specular.IdealSpecularReflect(r.Direction, n);
							r = new Ray(p, d, Sphere.EpsilonSphere, double.PositiveInfinity, r.Depth + 1);
							break;
						}
					case Sphere.MaterialType.Refractive: {
							var d = Specular.IdealSpecularTransmit(r.Direction, n, RefractiveIndexOut, RefractiveIndexIn, out var pr, rng);
							f *= pr;
							r = new Ray(p, d, Sphere.EpsilonSphere, double.PositiveInfinity, r.Depth + 1);
							break;
						}
					default: {
							var w = n.Dot(r.Direction) < 0 ? n : -n;
							var u = ((Math.Abs(w.X) > 0.1 ? new Vector3(0.0, 1.0, 0.0) : new Vector3(1.0, 0.0, 0.0)).Cross(w)).Normalize();
							var v = w.Cross(u);

							var sample_d = Sampling.CosineWeightedSampleOnHemisphere(rng.UniformFloat(), rng.UniformFloat());
							var d = (sample_d.X * u + sample_d.Y * v + sample_d.Z * w).Normalize();
							r = new Ray(p, d, Sphere.EpsilonSphere, double.PositiveInfinity, r.Depth + 1);
							break;
						}
				}
			}
		}
		
    }
}