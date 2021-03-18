using Listener.Plugin.ChromaEffect.Extensions;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualGrid;
using VirtualGrid.Enums;
using VirtualGrid.Interfaces;

namespace Listener.Plugin.ChromaEffect.Effects
{
    public class VisualizeVolumeChroma : IChromaEffect
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
