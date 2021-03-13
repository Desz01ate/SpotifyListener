using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.ChromaEffect.Implementation
{
    public partial struct Color
    {
        /// <summary>
        /// Gets the blue component of the color as a byte.
        /// </summary>
        public byte B => (byte)((Value >> 16) & 0xFF);

        /// <summary>
        /// Gets the green component of the color as a byte.
        /// </summary>
        public byte G => (byte)((Value >> 8) & 0xFF);

        /// <summary>
        /// Gets the red component of the color as a byte.
        /// </summary>
        public byte R => (byte)(Value & 0xFF);

        /// <summary>
        /// Gets the unsigned integer representing
        /// the color. On the form <c>0xKKBBGGRR</c>.
        /// </summary>
        public readonly uint Value;

        public Color(uint value)
        {
            Value = value;
        }

        public Color(byte red, byte green, byte blue) : this(red + ((uint)green << 8) + ((uint)blue << 16))
        {
        }

        public Color(float red, float green, float blue) : this((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255))
        {
        }

        public Color(double red, double green, double blue) : this((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255))
        {
        }

        public static Color FromRgb(uint value)
        {
            return new Color(((value & 0xFF0000) >> 16) | (value & 0xFF00) | ((value & 0xFF) << 16));
        }

        public override string ToString() => $"{R}, {G}, {B} (0x{Value:X7})";
    }
}
