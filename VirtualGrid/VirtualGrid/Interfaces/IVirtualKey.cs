using System;
using System.Collections.Generic;
using System.Text;
using VirtualGrid.Enums;

namespace VirtualGrid.Interfaces
{
    /// <summary>
    /// Virtual key that represent a physical key in virtual grid.
    /// </summary>
    public interface IVirtualKey

    {
        /// <summary>
        /// Index of key in virtual grid.
        /// </summary>
        (int X, int Y) Index { get; }

        /// <summary>
        /// Friendly name.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Key type.
        /// </summary>
        KeyType Type { get; }

        /// <summary>
        /// Enum value of key.
        /// </summary>
        int KeyCode { get; }

        /// <summary>
        /// Color of key.
        /// </summary>
        Color Color { get; set; }
    }
}
