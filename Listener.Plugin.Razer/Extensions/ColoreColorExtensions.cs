using System;
using System.Drawing;
using System.Linq;
using ColoreColor = Colore.Data.Color;

namespace Listener.Plugin.Razer.Extensions
{
    public static class ColoreColorExtensions
    {
        public static ColoreColor ChangeBrightnessLevel(this ColoreColor c, double multiplier)
        {
            if (multiplier == 1)
                return c;
            if (multiplier <= 0)
                return ColoreColor.Black;
            var R = (byte)(c.R * multiplier);
            var G = (byte)(c.G * multiplier);
            var B = (byte)(c.B * multiplier);
            return FromRgb(R, G, B);
        }

        public static ColoreColor FromRgb(byte r, byte g, byte b)
        {
            var hex = $"0x{r:X2}{g:X2}{b:X2}";
            return ColoreColor.FromRgb(Convert.ToUInt32(hex, 16));
        }
    }

}
