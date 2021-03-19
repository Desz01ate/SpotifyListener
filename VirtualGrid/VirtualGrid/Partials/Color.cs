using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualGrid
{
    public partial struct Color
    {
        /// <summary>
        /// Black color.
        /// </summary>
        public static readonly Color Black = new Color(0, 0, 0);

        /// <summary>
        /// (Dark) blue color.
        /// </summary>
        public static readonly Color Blue = new Color(0, 0, 255);

        /// <summary>
        /// (Neon/bright) green color.
        /// </summary>
        public static readonly Color Green = new Color(0, 255, 0);

        /// <summary>
        /// Hot pink color.
        /// </summary>
        public static readonly Color HotPink = new Color(255, 105, 180);

        /// <summary>
        /// Orange color.
        /// </summary>
        public static readonly Color Orange = FromRgb(0xFFA500);

        /// <summary>
        /// Pink color.
        /// </summary>
        public static readonly Color Pink = new Color(255, 0, 255);
        /// <summary>
        /// Purple color.
        /// </summary>

        public static readonly Color Purple = FromRgb(0x800080);
        /// <summary>
        /// Red color.
        /// </summary>
        public static readonly Color Red = new Color(255, 0, 0);

        /// <summary>
        /// White color.
        /// </summary>
        public static readonly Color White = new Color(255, 255, 255);

        /// <summary>
        /// Yellow color.
        /// </summary>
        public static readonly Color Yellow = new Color(255, 255, 0);
    }

}
