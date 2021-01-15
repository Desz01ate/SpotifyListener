using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.ImageProcessing
{
    public static class ColorProcessing
    {
        public static IEnumerable<Tuple<int, int, int>> GenerateRainbowSinusoidal(int range = 64)
        {
            const double frequency = .2;
            const int amplitude = 127;
            const int center = 128;

            //const double frequency = .1;
            //const int amplitude = 127;
            //const int center = 128;

            var phase1 = 0;
            var phase2 = 2 * Math.PI / 3;
            var phase3 = 4 * Math.PI / 3;

            for (var i = 0; i < range; ++i)
            {
                var r = Math.Sin(frequency * i + phase1) * amplitude + center;
                var g = Math.Sin(frequency * i + phase2) * amplitude + center;
                var b = Math.Sin(frequency * i + phase3) * amplitude + center;
                yield return new Tuple<int, int, int>((int)r, (int)g, (int)b);
            }
        }

        public static IEnumerable<Color> GenerateGradients(IEnumerable<Color> colors, bool rollOver = false)
        {
            var buffer = colors.ToArray();
            if (rollOver)
            {
                foreach (var color in GenerateRollOverGrandients(buffer))
                    yield return color;
                yield break;
            }
            var step = 128 / buffer.Length;
            for (var c = 0; c < buffer.Length - 1; c++)
            {
                var c1 = buffer[c];
                var c2 = buffer[c + 1];
                foreach (var color in GenerateGradients(c1, c2, step))
                {
                    yield return color;
                }
            }
        }

        private static IEnumerable<Color> GenerateRollOverGrandients(Color[] colors)
        {
            var buffer = new List<Color>();
            var step = 128 / colors.Length;
            for (var c = 0; c < colors.Length - 1; c++)
            {
                var c1 = colors[c];
                var c2 = colors[c + 1];
                foreach (var color in GenerateGradients(c1, c2, step))
                {
                    buffer.Add(color);
                    yield return color;
                }
            }
            buffer.Reverse();
            foreach (var color in buffer)
                yield return color;
        }

        private static IEnumerable<Color> GenerateGradients(Color start, Color end, int steps)
        {
            int stepA = ((end.A - start.A) / (steps - 1));
            int stepR = ((end.R - start.R) / (steps - 1));
            int stepG = ((end.G - start.G) / (steps - 1));
            int stepB = ((end.B - start.B) / (steps - 1));

            for (int i = 0; i < steps; i++)
            {
                yield return Color.FromArgb(start.A + (stepA * i),
                                            start.R + (stepR * i),
                                            start.G + (stepG * i),
                                            start.B + (stepB * i));
            }
        }

        public static Color InverseColor(this Color c)
        {
            return Color.FromArgb((int)(Color.FromArgb(c.R, c.G, c.B).ToArgb() ^ 0xFFFFFFFu));
        }

        public static Color ChangeBrightnessLevel(this Color c, double multiplier, double alpha = 255)
        {
            if (multiplier == 1 && alpha == 255)
                return c;
            if (multiplier <= 0)
                return Color.Black;
            return Color.FromArgb((byte)alpha, (byte)(c.R * multiplier), (byte)(c.G * multiplier), (byte)(c.B * multiplier));
        }

        public static Color ContrastColor(this Color c)
        {
            int d = 0;
            double a = 1 - (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;
            if (a >= 0.5)
                d = 255;
            return Color.FromArgb(d, d, d);
        }


        public static string ToHex(this Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        public static uint ToUint(this Color c)
        {
            return (uint)(((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B) & 0xffffffffL);
        }
    }
}
