using Listener.Plugin.ChromaEffect.Extensions;
using Listener.Plugin.ChromaEffect.Implementation;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.ChromaEffect.Effects
{
    public class VisualizeVolumeAlbumArtwork : IChromaEffect
    {
        public string EffectName => "Album Cover - Spectrum Visualizer";

        public int RequiredSpectrumRange => 29;

        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            if (albumArtworkColor == null)
                return;
            SetGridBackground(virtualGrid, albumArtworkColor, brightnessMultiplier);

            for (var x = 0; x < virtualGrid.ColumnCount; x++)
            {
                var foreground = albumColor.ElementAt(albumColor.Count - 1 - x);

                var c = spectrumValues[x];
                var absSpectrum = virtualGrid.RowCount - (int)Math.Round((virtualGrid.RowCount * (c / 100.0d)), 0);
                for (var y = virtualGrid.RowCount - 1; y >= absSpectrum; y--)
                {
                    virtualGrid[x, y] = foreground;
                }
            }
        }

        private void SetGridBackground(IVirtualLedGrid grid, Color[][] colorMap, double brightness)
        {
            for (var y = 0; y < colorMap.GetLength(0); y++)
            {
                var row = colorMap[y];
                for (var x = 0; x < row.Length; x++)
                {
                    grid[x, y] = row[x].ChangeBrightnessLevel(brightness);
                }
            }
        }
    }
}
