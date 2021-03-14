using Listener.Plugin.ChromaEffect.Extensions;
using Listener.Plugin.ChromaEffect.Implementation;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Rgb.Explodier
{
    public class ExplodierEffect : IChromaEffect
    {
        public string EffectName => "Explodier";

        public int RequiredSpectrumRange => 1;

        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            virtualGrid.Set(Color.Black);
            var fullXLength = virtualGrid.RowCount;
            var fullYLength = virtualGrid.ColumnCount;
            var volume = spectrumValues[0] / 100.0;
            var startXPosition = fullXLength / 2;
            var startYPosition = fullYLength / 2;
            var absolteXPosition = (int)Math.Floor(((volume * fullXLength) / 2) + 0.5);
            var absoluteYPosition = (int)Math.Floor(((volume * fullYLength) / 2) + 0.5);
            var upperXBound = startXPosition + absolteXPosition;
            var upperYbound = startYPosition + absoluteYPosition;
            for (var row = startXPosition; row < upperXBound; row++)
            {
                for (var col = startYPosition; col < upperYbound; col++)
                {
                    var color = Listener.Plugin.ChromaEffect.Shared.SharedColors.RainbowColor.ElementAt(upperYbound - col).ChangeBrightnessLevel(volume);
                    var extendCol = fullYLength - col - 1;
                    var extendRow = fullXLength - row - 1;
                    virtualGrid[col, row] = color;
                    virtualGrid[extendCol, row] = color;
                    virtualGrid[col, extendRow] = color;
                    virtualGrid[extendCol, extendRow] = color;
                }
            }
        }
    }
}
