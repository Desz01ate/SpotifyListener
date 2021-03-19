using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualGrid.Enums
{
    /// <summary>
    /// Type of key in virtual grid.
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// Keyboard key.
        /// </summary>
        Keyboard,

        /// <summary>
        /// Mouse key.
        /// </summary>
        Mouse,

        /// <summary>
        /// Mousepad LED.
        /// </summary>
        Mousepad,

        /// <summary>
        /// Headset LED.
        /// </summary>
        Headset,

        /// <summary>
        /// Extra device LED.
        /// </summary>
        ExtraDevice,

        /// <summary>
        /// Invalid key, use to indicate empty key on the grid.
        /// </summary>
        Invalid
    }
}
