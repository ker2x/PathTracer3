﻿using System;

namespace PathTracer3
{
    public static class MathUtils
    {

        public static double Clamp(double x, double low = 0.0, double high = 1.0) => 
            x < high ? x > low ? x : low : high;

        public static byte ToByte(double x, double gamma = 2.2) => 
            (byte) Clamp(255.0 * Math.Pow(x, 1 / gamma), 0.0, 255.0);
    }
}