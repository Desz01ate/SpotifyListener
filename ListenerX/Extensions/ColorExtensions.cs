using System.Drawing;
using System.Linq;
using ChromaColor = Listener.Plugin.ChromaEffect.Implementation.Color;

namespace ListenerX.Extensions
{
    public static class ColorExtensions
    {
        public static ChromaColor ComplementColor(this ChromaColor c)
        {
            return Color.FromArgb((int)(Color.FromArgb(c.R, c.G, c.B).ToArgb() ^ 0xFFFFFFFu)).ToChromaColor();
        }
        public static Color ChangeBrightnessLevel(this Color c, double multiplier, double alpha = 255)
        {
            return Color.FromArgb((byte)alpha, (byte)(c.R * multiplier), (byte)(c.G * multiplier), (byte)(c.B * multiplier));
        }
        public static ChromaColor ChangeBrightnessLevel(this ChromaColor c, double multiplier)
        {
            if (multiplier == 1)
                return c;
            if (multiplier <= 0)
                return ChromaColor.Black;
            var R = (byte)(c.R * multiplier);
            var G = (byte)(c.G * multiplier);
            var B = (byte)(c.B * multiplier);
            return Color.FromArgb(R, G, B).ToChromaColor();
        }
        public static ChromaColor ToChromaColor(this Color c)
        {
            return new ChromaColor(c.R, c.G, c.B);
        }

        public static ChromaColor[][] ToChromaColors(this Color[][] colors)
        {
            return colors.Select(row => row.Select(key => key.ToChromaColor()).ToArray()).ToArray();
        }

        public static ChromaColor ContrastColor(this ChromaColor c)
        {
            ChromaColor color = ChromaColor.Black;
            double a = 1 - (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;
            if (a >= 0.5)
                color = ChromaColor.White;
            return color;
        }

        public static string ToHex(this ChromaColor c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
    }
}
