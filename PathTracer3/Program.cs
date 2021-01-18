using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PathTracer3
{
    internal static class Program
    {
		private static void Main(string[] args)
		{
			const int width = 1024;
			const int height = 1024;
			const int defaultSample = 64;
			const double fov = 0.5135;

			var startTime = Stopwatch.StartNew();
			var rng = new Rng();
			var nbSamples = (args.Length > 0) ? int.Parse(args[0]) / 4 : defaultSample;
			
			Console.WriteLine($"Starting {DateTime.Now}");

			var eye = new Vector3(50, 52, 295.6);
			var gaze = new Vector3(0, -0.042612, -1).Normalize();
			var cx = new Vector3(width * fov / height, 0.0, 0.0);
			var cy = (cx.Cross(gaze)).Normalize() * fov;
			var vList = new Vector3[width * height];
			
			for (var index = 0; index < width * height; ++index) {
				vList[index] = new Vector3();
			}

			Parallel.For(0, height, y => RenderFunc(y, width, height, nbSamples, eye, gaze, cx, cy, vList, rng));
			//Uncomment for single thread
			/*for (int i = 0; i < height; ++i)
			{
				RenderFunc(i, width, height, nbSamples, eye, gaze, cx, cy, vList, rng);
			}
			*/
			ImageIO.WritePPM(width, height, vList);
			Console.WriteLine($"\nRun time : {(startTime.Elapsed)}");
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private static void RenderFunc(int y, int w, int h, int nbSamples,
			                           Vector3 eye, Vector3 gaze,
									   Vector3 cx,  Vector3 cy,
									   Vector3[] vList, Rng rng) {

			var luminance = new Vector3();
			for (var x = 0; x < w; ++x) {									// row
				for (int sy = 0, i = (h - 1 - y) * w + x; sy < 2; ++sy) {	//column
					for (var sx = 0; sx < 2; ++sx) {						//subpixel row
						luminance.Zero();
						for (var s = 0; s < nbSamples; ++s) {				//subpixel column
							var u1 = 2.0 * rng.UniformFloat();
							var u2 = 2.0 * rng.UniformFloat();
							var dx = u1 < 1 ? Math.Sqrt(u1) - 1.0 : 1.0 - Math.Sqrt(2.0 - u1);
							var dy = u2 < 1 ? Math.Sqrt(u2) - 1.0 : 1.0 - Math.Sqrt(2.0 - u2);
							var d = cx * (((sx + 0.5 + dx) / 2 + x) / w - 0.5) +
							        cy * (((sy + 0.5 + dy) / 2 + y) / h - 0.5) + gaze;
							luminance += Radiance(new Ray(eye + d * 130, d.Normalize(), Sphere.EpsilonSphere), rng) * (1.0 / nbSamples);
						}
						vList[i] += 0.25 * Vector3.Clamp(luminance);
					}
				}
			}
		}

		// Scene
		private const double RefractiveIndexOut = 1.0;
		private const double RefractiveIndexIn  = 1.5;

		// ReSharper disable once HeapView.ObjectAllocation.Evident
		private static readonly Sphere[] Spheres =
		{
			new(1e5,  new Vector3(1e5 + 1, 40.8, 81.6),   new Vector3(),   new Vector3(0.75,0.25,0.25),   Sphere.MaterialType.Diffuse),    //Left
			new(1e5,  new Vector3(-1e5 + 99, 40.8, 81.6), new Vector3(),   new Vector3(0.25,0.25,0.75),   Sphere.MaterialType.Diffuse),	//Right
			new(1e5,  new Vector3(50, 40.8, 1e5),         new Vector3(),   new Vector3(0.25, 0.75, 0.25), Sphere.MaterialType.Diffuse),	//Back
			new(1e5,  new Vector3(50, 40.8, -1e5 + 170),  new Vector3(),   new Vector3(),                      Sphere.MaterialType.Diffuse),	//Front
			new(1e5,  new Vector3(50, 1e5, 81.6),         new Vector3(),   new Vector3(0.75),                Sphere.MaterialType.Diffuse),    //Bottom
			new(1e5,  new Vector3(50, -1e5 + 81.6, 81.6), new Vector3(),   new Vector3(0.75),                Sphere.MaterialType.Diffuse),	//Top
			new(16.5, new Vector3(27, 16.5, 47),          new Vector3(),   new Vector3(0.499),               Sphere.MaterialType.Specular),	//Mirror
			new(6.5, new Vector3(50, 16.5, 97),          new Vector3(),   new Vector3(0.5,0.5,0.999),               Sphere.MaterialType.Refractive),	//Glass
//			new(3.5, new Vector3(50, 36.5, 157),          new Vector3(),   new Vector3(0.9),               Sphere.MaterialType.Specular),	//Mirror
			new(16.5, new Vector3(73, 16.5, 78),          new Vector3(),   new Vector3(0.999),               Sphere.MaterialType.Refractive),	//Glass
			new(600,  new Vector3(50, 681.6 - .27, 81.6), new Vector3(12,12,5), new Vector3(),                    Sphere.MaterialType.Diffuse)		//Light
		};

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private static bool Intersect(ref Ray ray, out int id) {
			id = 0;
			var hit = false;
			
			for (var i = 0; i < Spheres.Length; ++i)
			{
				if (!Spheres[i].Intersect(ref ray)) continue;
				hit = true;
				id = i;
			}
			return hit;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
		[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
		public static bool Intersect(Ray ray) => 
			Spheres.Any(t => t.Intersect(ref ray));

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private static Vector3 Radiance(Ray ray, Rng rng) {
			var luminance = new Vector3();
			var color = new Vector3(1.0);

			while (true) {
				if (!Intersect(ref ray, out var id)) {
					return luminance;
				}

				var shape = Spheres[id];
				var p = ray.Eval(ray.Tmax);
				var n = (p - shape.Position).Normalize();

				luminance += color * shape.Emission;
				color *= shape.Color;

				// Russian roulette
				if (ray.Depth > 4) {
					var continueProbability = shape.Color.Max();
					if (rng.UniformFloat() >= continueProbability) {
						return luminance;
					}
					color /= continueProbability;
				}

				// Next path segment
				switch (shape.Material) {
					case Sphere.MaterialType.Specular: {
							var d = Specular.IdealSpecularReflect(ray.Direction, n);
							ray = new Ray(p, d, Sphere.EpsilonSphere, double.PositiveInfinity, ray.Depth + 1);
							break;
						}
					case Sphere.MaterialType.Refractive: {
							var d = Specular.IdealSpecularTransmit(ray.Direction, n, RefractiveIndexOut, RefractiveIndexIn, out var pr, rng);
							color *= pr;
							ray = new Ray(p, d, Sphere.EpsilonSphere, double.PositiveInfinity, ray.Depth + 1);
							break;
						}
					default: {
							var w = n.Dot(ray.Direction) < 0 ? n : -n;
							var u = ((Math.Abs(w.X) > 0.1 ? new Vector3(0.0, 1.0, 0.0) : new Vector3(1.0, 0.0, 0.0)).Cross(w)).Normalize();
							var v = w.Cross(u);

							var sampleDistance = Sampling.CosineWeightedSampleOnHemisphere(rng.UniformFloat(), rng.UniformFloat());
							var distance = (sampleDistance.X * u + sampleDistance.Y * v + sampleDistance.Z * w).Normalize();
							ray = new Ray(p, distance, Sphere.EpsilonSphere, double.PositiveInfinity, ray.Depth + 1);
							break;
						}
				}
			}
		}
		
    }
}