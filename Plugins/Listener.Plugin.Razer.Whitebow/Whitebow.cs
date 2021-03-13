using Listener.Plugin.ChromaEffect.Extensions;
using Listener.Plugin.ChromaEffect.Implementation;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Razer.Whitebow
{
    public class Whitebow : IChromaEffect
    {
        public string EffectName => "Whitebow Effect";

        public int RequiredSpectrumRange => 29;

        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            var colors = Listener.Plugin.ChromaEffect.Shared.SharedColors.RainbowColor;
            var visualizeRowCount = virtualGrid.RowCount - 1;
            for (var col = 0; col < virtualGrid.ColumnCount; col++)
            {
                var foreground = colors.ElementAt(colors.Count - 1 - col);
                var background = foreground.ChangeBrightnessLevel(brightnessMultiplier);
                foreach (var key in virtualGrid.Where(e => e.Index.X == col))
                {
                    key.Color = background;
                }

                var c = spectrumValues[col];
                var absSpectrum = virtualGrid.RowCount - (int)Math.Round((visualizeRowCount * (c / 100.0d)), 0);
                for (var row = virtualGrid.RowCount - 1; row >= absSpectrum; row--)
                {
                    virtualGrid[col, row] = Color.White;
                }
            }

            var avgVolume = spectrumValues.Average() / 100.0;
            var startPosition = virtualGrid.ColumnCount / 2;
            var absolutePosition = (int)Math.Round((avgVolume * virtualGrid.ColumnCount), 0) / 2;
            var upperbound = startPosition + absolutePosition;
            for (var col = startPosition; col < upperbound; col++)
            {
                var color = colors.ElementAt(upperbound - col).ChangeBrightnessLevel(1);
                var left = col;
                var right = virtualGrid.ColumnCount - col - 1;
                virtualGrid[left, 0] = color;
                virtualGrid[right, 0] = color;
            }
        }
    }
}
