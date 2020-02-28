using ColorThiefDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using ColoreColor = Colore.Data.Color;
using ThiefColor = ColorThiefDotNet.Color;
namespace NListener.Core.Foundation.Helpers
{
    public static class BitmapHelper
    {
        private static readonly ColorThief picker = new ColorThief();
        public static (byte red, byte green, byte blue) DominantColor(this Bitmap img)
        {
            var dominant = picker.GetColor(img);
            return (dominant.Color.R, dominant.Color.G, dominant.Color.B);
        }
        public static (byte red, byte green, byte blue) AverageColor(this Bitmap img)
        {
            using (var bitmap = img)
            {
                var startX = 0;
                var startY = 0;
                byte r = 0, g = 0, b = 0, total = 0;
                for (int x = startX; x < bitmap.Size.Width; x++)
                {
                    for (int y = startY; y < bitmap.Size.Height; y++)
                    {
                        System.Drawing.Color clr = bitmap.GetPixel(x, y);
                        r += clr.R;
                        g += clr.G;
                        b += clr.B;
                        total++;
                    }
                }
                //Calculate average
                r /= total;
                g /= total;
                b /= total;
                return (r, g, b);
            }
        }
        public static Color InverseColor(this Color c)
        {
            return Color.FromArgb((int)(Color.FromArgb(c.R, c.G, c.B).ToArgb() ^ 0xFFFFFFFu));
        }
        public static ColoreColor InverseColor(this ColoreColor c)
        {
            return Color.FromArgb((int)(Color.FromArgb(c.R, c.G, c.B).ToArgb() ^ 0xFFFFFFFu)).ToColoreColor();
        }
        public static ColoreColor ToColoreColor(this Color color)
        {
            return new ColoreColor(color.R, color.G, color.B);
        }
    }
}
