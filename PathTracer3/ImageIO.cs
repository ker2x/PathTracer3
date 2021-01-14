using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PathTracer3
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ImageIO
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "HeapView.ObjectAllocation.Evident")]
        [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
        public static void WritePPM(int w, int h, Vector3[] Ls, string fileName = "cs-image.ppm")
        {
            using var sw = new StreamWriter(fileName);
            sw.Write($"P3\n{w} {h}\n{255}\n");
            for (var index = 0; index < w * h; ++index)
                sw.Write($"{MathUtils.ToByte(Ls[index].X)} {MathUtils.ToByte(Ls[index].Y)} {MathUtils.ToByte(Ls[index].Z)} ");
        }
    }
}