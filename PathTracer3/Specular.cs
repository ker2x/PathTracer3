using System;

namespace PathTracer3
{
    public static class Specular
    {
        private static double Reflectance0(double n1, double n2) => 
            ((n1 - n2) / (n1 + n2)) * ((n1 - n2) / (n1 + n2));

        private static double SchlickReflectance(double n1, double n2, double c)
        {
            var r0 = Reflectance0(n1, n2);
            return r0 + (1 - r0) * c * c * c * c * c;
        }

        public static Vector3 IdealSpecularReflect(Vector3 d, Vector3 n) => d - 2.0 * n.Dot(d) * n;

        public static Vector3 IdealSpecularTransmit(Vector3 d, Vector3 n, double nOut, double nIn, 
            out double pr, Rng rng)
        {
            var dRe = IdealSpecularReflect(d, n);

            var outToIn = n.Dot(d) < 0;
            var nl = outToIn ? n : -n;
            var nn = outToIn ? nOut / nIn : nIn / nOut;
            var cosTheta = d.Dot(nl);
            var cos2Phi = 1.0 - nn * nn * (1.0 - cosTheta * cosTheta);

            // Total Internal Reflection
            if (cos2Phi < 0)
            {
                pr = 1.0;
                return dRe;
            }

            var dTr = (nn * d - nl * (nn * cosTheta + Math.Sqrt(cos2Phi))).Normalize();
            var c = 1.0 - (outToIn ? -cosTheta : dTr.Dot(n));

            var reflectance = SchlickReflectance(nOut, nIn, c);
            var pRe = 0.25 + 0.5 * reflectance;
            if (rng.UniformFloat() < pRe)
            {
                pr = reflectance / pRe;
                return dRe;
            }

            var transmittance = 1.0 - reflectance;
            var pTransmittance = 1.0 - pRe;
            pr = transmittance / pTransmittance;
            return dTr;
        }
    }
}