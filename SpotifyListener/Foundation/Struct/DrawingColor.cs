using Listener.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.Foundation.Struct
{
    public struct DrawingColor
    {
        public Color Based { get; private set; }
        public Color Complemented { get; private set; }
        public DrawingColor(int r, int g, int b)
        {
            Based = Color.FromArgb(r, g, b);
            Complemented = Based.InverseColor();
        }
        public DrawingColor(int alpha, Color baseColor)
        {
            Based = Color.FromArgb(alpha, baseColor);
            Complemented = Based.InverseColor();
        }
        public DrawingColor(int a, int r, int g, int b)
        {
            Based = Color.FromArgb(a, r, g, b);
            Complemented = Based.InverseColor();
        }
        public DrawingColor(int argb)
        {
            Based = Color.FromArgb(argb);
            Complemented = Based.InverseColor();
        }
    }

}
