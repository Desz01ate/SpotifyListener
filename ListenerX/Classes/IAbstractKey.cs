using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Classes
{
    public enum KeyType
    {
        Keyboard,
        Mouse,
        Mousepad,
        Headset,
        ChromaLink,
        Invalid
    }
    public interface IAbstractKey
    {
        public (int X, int Y) Index { get; }

        public string FriendlyName { get; }

        public KeyType Type { get; }

        public int internalKeyCode { get; }

        public Colore.Data.Color Color { get; set; }
    }
}
