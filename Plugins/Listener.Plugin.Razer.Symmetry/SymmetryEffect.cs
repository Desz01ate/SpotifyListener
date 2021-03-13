using Listener.Plugin.ChromaEffect.Extensions;
using Listener.Plugin.ChromaEffect.Implementation;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Razer.Symmetry
{
    public class SymmetryEffect : IChromaEffect
    {
        public string EffectName => "Symmetry Chroma";

        public int RequiredSpectrumRange => 1;

        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            virtualGrid.Set(Color.Black);
            var fullLength = virtualGrid.ColumnCount;
            var volume = spectrumValues[0] / 100.0;
            var startPosition = (fullLength / 2);
            var absolutePosition = (int)Math.Round((volume * fullLength), 0) / 2;
            var upperbound = startPosition + absolutePosition;
            for (var row = 0; row < virtualGrid.RowCount; row++)
            {
                for (var col = startPosition; col < upperbound; col++)
                {
                    var color = Listener.Plugin.ChromaEffect.Shared.SharedColors.RainbowColor.ElementAt(upperbound - col).ChangeBrightnessLevel(volume);
                    virtualGrid[col, row] = color;
                    virtualGrid[fullLength - col - 1, row] = color;
                }
            }
        }
    }
}
