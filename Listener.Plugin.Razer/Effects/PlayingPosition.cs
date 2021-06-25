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
    public class PlayingPosition : IChromaEffect
    {
        public string EffectName => "Album Cover - Progression + Volume (Gradient)";

        public int RequiredSpectrumRange => 6;

        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            if (double.IsNaN(position) || double.IsInfinity(position))
                return;

            for (var x = 0; x < virtualGrid.ColumnCount; x++)
            {
                var background = albumColor.ElementAt(x).ChangeBrightnessLevel(brightnessMultiplier);
                foreach (var key in virtualGrid.Where(e => e.Index.X == x))
                {
                    key.Color = background;
                }
            }

            var keyboardGrid = virtualGrid;
            var keyboardRowCount = keyboardGrid.Max(e => e.Index.Y) + 1;
            for (var rowIdx = 0; rowIdx < keyboardRowCount; rowIdx++)
            {
                var row = keyboardGrid.Where(e => e.Index.Y == rowIdx && e.Index.X < 22).ToArray();
                var pos = (int)Math.Round(position * ((double)(row.Length - 1) / 10), 0);
                var key = row[pos];
                virtualGrid[key.Index.X, key.Index.Y] = firstColor;
                if (0 < pos - 1 && pos + 1 < row.Length)
                {
                    var leftKey = row[pos - 1];
                    var rightKey = row[pos + 1];

                    var adjacentColor = firstColor.ChangeBrightnessLevel(0.5);
                    virtualGrid[leftKey.Index.X, leftKey.Index.Y] = adjacentColor;
                    virtualGrid[rightKey.Index.X, rightKey.Index.Y] = adjacentColor;
                }
            }

            var maxMouseY = virtualGrid.Where(x => 22 < x.Index.X && x.Index.X < 29).Max(x => x.Index.Y) + 1;
            var currentPlayPosition = (int)Math.Round(position * ((double)(maxMouseY - 1) / 10), 0);
            virtualGrid[22, currentPlayPosition] = firstColor;

            var vizColor = albumColor.First();
            for (var x = 23; x < 29; x++)
            {
                var volume = spectrumValues[x - 23];
                var absPosition = maxMouseY - (int)Math.Round((volume / 100d) * maxMouseY, 0);
                for (var y = maxMouseY - 1; y >= absPosition; y--)
                {
                    virtualGrid[x, y] = vizColor;
                }
            }
        }
    }
}
