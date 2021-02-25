using Colore.Data;
using Listener.Plugin.Razer.Extensions;
using Listener.Plugin.Razer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Razer.Whitebow
{
    public class Whitebow : IRazerEffect
    {
        public string EffectName => "Whitebow Effect";

        public int RequiredSpectrumRange => 29;

        public void SetEffect(global::Colore.Effects.Virtual.IVirtualLedGrid virtualGrid, global::Colore.Data.Color firstColor, global::Colore.Data.Color secondaryColor, ICollection<global::Colore.Data.Color> albumColor, global::Colore.Data.Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            var colors = Listener.Plugin.Razer.Shared.SharedColors.RainbowColor;
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
                    virtualGrid[x, y] = Color.White;
                }
            }
        }
    }
}
