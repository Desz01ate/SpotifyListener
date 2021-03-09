using Colore.Data;
using Listener.Core.Framework.DataStructure;
using Listener.Core.Framework.Extensions;
using Listener.Plugin.Razer.Extensions;
using Listener.Plugin.Razer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Razer.CustomizableVisualizer
{
    public class CustomizableVisualizer : IRazerEffect
    {
        public string EffectName => "Spectrum Visualizer (Customizable)";

        public int RequiredSpectrumRange => 29;

        private readonly IReadOnlyCollection<Color> foreground, background;
        public CustomizableVisualizer()
        {
            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            var configFile = Path.Combine(pluginDir, "customvisual.cfg");
            if (File.Exists(configFile))
            {
                var config = File.ReadAllLines(configFile);
                foreach (var line in config)
                {
                    var split = line.Split(' ');
                    var key = split[0].ToLower();
                    var value = split[1].ToLower();
                    switch (key)
                    {
                        case "foreground":
                            if (value == "rainbow")
                                foreground = Listener.Plugin.Razer.Shared.SharedColors.RainbowColor;
                            else
                            {
                                var rgb = value.Split(',').Select(x => x.Trim()).ToArray();
                                var r = rgb[0];
                                var g = rgb[1];
                                var b = rgb[2];
                                foreground = (new AutoshiftCirculaQueue<Color>(Enumerable.Range(0, 64).Select(_ => ColoreColorExtensions.FromRgb(byte.Parse(r), byte.Parse(g), byte.Parse(b))), 500)).AsReadOnly();
                            }
                            break;
                        case "background":
                            if (value == "rainbow")
                                background = Listener.Plugin.Razer.Shared.SharedColors.RainbowColor;
                            else
                            {
                                var rgb = value.Split(',').Select(x => x.Trim()).ToArray();
                                var r = rgb[0];
                                var g = rgb[1];
                                var b = rgb[2];
                                background = (new AutoshiftCirculaQueue<Color>(Enumerable.Range(0, 64).Select(_ => ColoreColorExtensions.FromRgb(byte.Parse(r), byte.Parse(g), byte.Parse(b))), 500)).AsReadOnly();
                            }
                            break;
                        default:
                            goto Fallback;
                    }
                }
            }
            return;
        Fallback:
            foreground = (new AutoshiftCirculaQueue<Color>(Enumerable.Range(0, 64).Select(_ => Color.White), 500)).AsReadOnly();
            background = (new AutoshiftCirculaQueue<Color>(Enumerable.Range(0, 64).Select(_ => Color.Black), 500)).AsReadOnly();
        }

        public void SetEffect(global::Colore.Effects.Virtual.IVirtualLedGrid virtualGrid, global::Colore.Data.Color firstColor, global::Colore.Data.Color secondaryColor, ICollection<global::Colore.Data.Color> albumColor, global::Colore.Data.Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            for (var x = 0; x < virtualGrid.ColumnCount; x++)
            {
                var foreground = this.foreground.ElementAt(this.foreground.Count - 1 - x);
                var background = this.background.ElementAt(this.background.Count - 1 - x).ChangeBrightnessLevel(brightnessMultiplier);
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
