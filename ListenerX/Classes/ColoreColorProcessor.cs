using System.Drawing;
using System.Linq;
using ColoreColor = Colore.Data.Color;

namespace ListenerX
{
    static class ColoreColorProcessor
    {
        public static ColoreColor ComplementColor(this ColoreColor c)
        {
            return Color.FromArgb((int)(Color.FromArgb(c.R, c.G, c.B).ToArgb() ^ 0xFFFFFFFu)).ToColoreColor();
        }
        public static Color ChangeBrightnessLevel(this Color c, double multiplier, double alpha = 255)
        {
            return Color.FromArgb((byte)alpha, (byte)(c.R * multiplier), (byte)(c.G * multiplier), (byte)(c.B * multiplier));
        }
        public static ColoreColor ChangeBrightnessLevel(this ColoreColor c, double multiplier)
        {
            if (multiplier == 1)
                return c;
            if (multiplier <= 0)
                return ColoreColor.Black;
            var R = (byte)(c.R * multiplier);
            var G = (byte)(c.G * multiplier);
            var B = (byte)(c.B * multiplier);
            return Color.FromArgb(R, G, B).ToColoreColor();
        }
        public static ColoreColor ToColoreColor(this Color c)
        {
            return new ColoreColor(c.R, c.G, c.B);
        }

        public static ColoreColor[][] ToColoreColors(this Color[][] colors)
        {
            return colors.Select(row => row.Select(key => key.ToColoreColor()).ToArray()).ToArray();
        }

        public static ColoreColor ContrastColor(this ColoreColor c)
        {
            ColoreColor color = ColoreColor.Black;
            double a = 1 - (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;
            if (a >= 0.5)
                color = ColoreColor.White;
            return color;
        }

        public static ColoreColor SoftColor(this ColoreColor c)
        {
            var rgb = new[] { c.R, c.G, c.B };
            if (rgb[0] == rgb[1] && rgb[1] == rgb[2])
            {
                return ContrastColor(c);
            }
            var max = rgb.Max();
            /* not the best code and also not a performance-wise, rather for a fucking lazy sake of me */
            for (var index = 0; index < rgb.Length; index++)
            {
                if (rgb[index] == max)
                {
                    var half = rgb[index] / 2;
                    if (index == 0)
                    {
                        var r = rgb[0];
                        var g = rgb[1] + half > 255 ? 255 : rgb[1] + half;
                        var b = rgb[2] + half > 255 ? 255 : rgb[2] + half;
                        return new ColoreColor(r, g, b);
                    }
                    else if (index == 1)
                    {
                        var r = rgb[0] + half > 255 ? 255 : rgb[0] + half;
                        var g = rgb[1];
                        var b = rgb[2] + half > 255 ? 255 : rgb[2] + half;
                        return new ColoreColor(r, g, b);
                    }
                    else if (index == 2)
                    {
                        var r = rgb[0] + half > 255 ? 255 : rgb[0] + half;
                        var g = rgb[1] + half > 255 ? 255 : rgb[1] + half;
                        var b = rgb[2];
                        return new ColoreColor(r, g, b);
                    }
                }
            }
            /* fallback */
            return c;
        }
    }
}
