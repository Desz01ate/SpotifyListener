using Listener.Plugin.ChromaEffect.Interfaces;
using System.Collections.Generic;
using System.Linq;
using VirtualGrid;
using VirtualGrid.Enums;
using VirtualGrid.Interfaces;

namespace Listener.Plugin.Razer.DebugGrid
{
    public class DebugGrid : IChromaEffect
    {
        public string EffectName => "Debug Grid";

        public int RequiredSpectrumRange => 29;

        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            virtualGrid.Set(Color.Black);

            for (var row = 0; row < virtualGrid.RowCount; row++)
            {
                for (var col = 0; col < virtualGrid.ColumnCount; col++)
                {
                    var segment = (float)col / virtualGrid.ColumnCount;
                    virtualGrid[col, row] = segment switch
                    {
                        var x when x <= 0.2 => Color.Red,
                        var x when x <= 0.4 => Color.Blue,
                        var x when x <= 0.6 => Color.Purple,
                        var x when x <= 0.8 => Color.Green,
                        _ => Color.White,
                    };
                }
            }
        }
    }
}
