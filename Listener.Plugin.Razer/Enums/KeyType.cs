using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.ChromaEffect.Enums
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
        /// Chroma Link virtual LED.
        /// </summary>
        ChromaLink,

        /// <summary>
        /// Invalid key, use to indicate empty key on the grid.
        /// </summary>
        Invalid
    }
}
