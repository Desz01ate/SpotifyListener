using Colore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NListener.Core.Foundation.Struct
{
    public struct RazerChromaColor
    {
        public Color Based { get; private set; }
        public Color Complemented { get; private set; }
        public RazerChromaColor(float r, float g, float b)
        {
            Based = new Color(r, g, b);
            Complemented = Based;
        }
        public RazerChromaColor(byte r, byte g, byte b)
        {
            Based = new Color(r, g, b);
            Complemented = Based;
        }
        public RazerChromaColor(double r, double g, double b)
        {
            Based = new Color(r, g, b);
            Complemented = Based;
        }
        public RazerChromaColor(uint value)
        {
            Based = new Color(value);
            Complemented = Based;
        }
    }
}
