using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoreColor = Colore.Data.Color;

namespace SpotifyListener.Interfaces
{
    public struct StandardColor
    {
        public Color Standard { get; set; }
        public Color Complemented { get; set; }
    }
    public struct DevicesColor
    {
        public ColoreColor Standard { get; set; }
        public ColoreColor Complemented { get; set; }
    }
    public interface IChromaRender
    {
        StandardColor Album_StandardColor { get; }
        DevicesColor Album_RazerColor { get; }
    }
}
