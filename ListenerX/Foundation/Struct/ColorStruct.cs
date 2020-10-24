using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoreColor = Colore.Data.Color;

namespace ListenerX.Foundation.Struct
{
    public struct StandardColor
    {
        public Color Standard { get; set; }
        public Color Complemented { get; set; }
    }
    public struct ChromaDevicesColor
    {
        public ColoreColor Standard { get; set; }
        public ColoreColor Complemented { get; set; }
    }
}
