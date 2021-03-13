using Listener.Plugin.ChromaEffect.Enums;
using Listener.Plugin.ChromaEffect.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.ChromaEffect.Interfaces
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
