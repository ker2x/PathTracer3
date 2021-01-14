using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PathTracer3
{
    public static class ImageIO
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void WritePPM(int w, int h, Vector3[] Ls, string fileName = "cs-image.ppm")
        {
            using (var sw = new StreamWriter(fileName))
            {
                var sbegin = string.Format("P3\n{0} {1}\n{2}\n", w, h, 255);
                sw.Write(sbegin);

                for (var i = 0; i < w * h; ++i)
                {
                    var s = string.Format("{0} {1} {2} ",
                        MathUtils.ToByte(Ls[i].X),
                        MathUtils.ToByte(Ls[i].Y),
                        MathUtils.ToByte(Ls[i].Z));
                    sw.Write(s);
                }
            }
        }
    }
}