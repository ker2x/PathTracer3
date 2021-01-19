using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace PathTracer3
{
    public class Rng
    {
        /* it look weird but this is the Rng constructor */
        [SuppressMessage("ReSharper", "HeapView.ObjectAllocation.Evident")] 
        private readonly ThreadLocal< Random > _rnd = new(
            () => new Random(Thread.CurrentThread.ManagedThreadId));

        public double UniformFloat() {
            return _rnd.Value.NextDouble();
        }
    }
}