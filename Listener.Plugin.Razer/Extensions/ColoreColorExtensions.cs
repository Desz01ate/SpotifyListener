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
            var hex = $"0x{R:X2}{G:X2}{B:X2}";
            return ColoreColor.FromRgb(Convert.ToUInt32(hex, 16));
        }
    }

}
