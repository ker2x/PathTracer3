using System;
using System.Threading;

namespace PathTracer3
{
    public class Rng
    {
        private readonly ThreadLocal< Random > _rnd = new(
            () => new Random(Thread.CurrentThread.ManagedThreadId));

        public double UniformFloat() {
            return _rnd.Value.NextDouble();
        }    }
}