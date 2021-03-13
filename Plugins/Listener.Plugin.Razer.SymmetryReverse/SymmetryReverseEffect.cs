using Listener.Plugin.ChromaEffect.Extensions;
using Listener.Plugin.ChromaEffect.Implementation;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Razer.SymmetryReverse
{
    public class SymmetryReverseEffect : IChromaEffect
    {
        public string EffectName => "Symmetry Chroma (Reverse)";

        public int RequiredSpectrumRange => 1;

        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            virtualGrid.Set(Color.Black);
            var fullLength = virtualGrid.ColumnCount;
            var volume = spectrumValues[0] / 100.0;
            var startPosition = 0;
            var absolutePosition = Math.Round((volume * fullLength), 0) / 2;
            for (var row = 0; row < virtualGrid.RowCount; row++)
            {
                for (var col = startPosition; col < startPosition + absolutePosition; col++)
                {
                    var color = Listener.Plugin.ChromaEffect.Shared.SharedColors.RainbowColor.ElementAt(col).ChangeBrightnessLevel(volume);
                    virtualGrid[col, row] = color;
                    virtualGrid[fullLength - col - 1, row] = color;
                }
            }
        }
    }
}
