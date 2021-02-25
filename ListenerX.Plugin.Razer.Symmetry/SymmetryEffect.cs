using Listener.Plugin.Razer.Extensions;
using Listener.Plugin.Razer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Plugin.Razer.Symmetry
{
    public class SymmetryEffect : IRazerEffect
    {
        public string EffectName => "Symmetry Chroma";

        public int RequiredSpectrumRange => 1;

        public void SetEffect(global::Colore.Effects.Virtual.IVirtualLedGrid virtualGrid, global::Colore.Data.Color firstColor, global::Colore.Data.Color secondaryColor, ICollection<global::Colore.Data.Color> albumColor, global::Colore.Data.Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            virtualGrid.Set(Colore.Data.Color.Black);
            var fullLength = virtualGrid.ColumnCount;
            var volume = spectrumValues[0] / 100.0;
            var startPosition = (fullLength / 2);
            var absolutePosition = Math.Round((volume * fullLength), 0) / 2;
            for (var row = 0; row < virtualGrid.RowCount; row++)
            {
                for (var col = startPosition; col < startPosition + absolutePosition; col++)
                {
                    var color = Listener.Plugin.Razer.Shared.SharedColors.RainbowColor.ElementAt(col).ChangeBrightnessLevel(volume);
                    virtualGrid[col, row] = color;
                    virtualGrid[fullLength - col - 1, row] = color;
                }
            }
        }
    }
}
