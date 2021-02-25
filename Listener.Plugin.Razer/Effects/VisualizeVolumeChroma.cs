using Colore.Data;
using Colore.Effects.Virtual;
using Listener.Plugin.Razer.Extensions;
using Listener.Plugin.Razer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Razer.Effects
{
    public class VisualizeVolumeChroma : IRazerEffect
    {
        // <inheritdoc/>
        public string EffectName => "Spectrum Visualizer (Chroma)";

        public int RequiredSpectrumRange => 29;

        // <inheritdoc/>
        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            var colors = Shared.SharedColors.RainbowColor;
            for (var x = 0; x < virtualGrid.ColumnCount; x++)
            {
                var foreground = colors.ElementAt(colors.Count - 1 - x);
                var background = foreground.ChangeBrightnessLevel(brightnessMultiplier);
                foreach (var key in virtualGrid.Where(e => e.Index.X == x))
                {
                    key.Color = background;
                }

                var c = spectrumValues[x];
                var absSpectrum = virtualGrid.RowCount - (int)Math.Round((virtualGrid.RowCount * (c / 100.0d)), 0);
                for (var y = virtualGrid.RowCount - 1; y >= absSpectrum; y--)
                {
                    virtualGrid[x, y] = foreground;
                }
            }
        }
    }
}
