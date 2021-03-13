using Listener.Plugin.ChromaEffect.Implementation;
using System;
using System.Drawing;
using System.Linq;

namespace Listener.Plugin.ChromaEffect.Extensions
{
    public static class ColorExtensions
    {
        public static Color ChangeBrightnessLevel(this Color c, double multiplier)
        {
            if (multiplier == 1)
                return c;
            if (multiplier <= 0)
                return Color.Black;
            var R = (byte)(c.R * multiplier);
            var G = (byte)(c.G * multiplier);
            var B = (byte)(c.B * multiplier);
            return FromRgb(R, G, B);
        }

        public static Color FromRgb(byte r, byte g, byte b)
        {
            var hex = $"0x{r:X2}{g:X2}{b:X2}";
            return Color.FromRgb(Convert.ToUInt32(hex, 16));
        }
    }

}
