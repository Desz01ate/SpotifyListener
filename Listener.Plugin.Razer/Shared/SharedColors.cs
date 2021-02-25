using Colore.Data;
using Listener.Core.Framework.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Razer.Shared
{
    public static class SharedColors
    {
        public static readonly AutoshiftCirculaQueue<Color> RainbowColor = new AutoshiftCirculaQueue<Color>(GenerateRainbowSinusoidal().Select(c => new Color(c.Item1, c.Item2, c.Item3)), 500);

        private static IEnumerable<Tuple<int, int, int>> GenerateRainbowSinusoidal(int range = 64)
        {
            const double frequency = .2;
            const int amplitude = 127;
            const int center = 128;

            //const double frequency = .1;
            //const int amplitude = 127;
            //const int center = 128;

            var phase1 = 0;
            var phase2 = 2 * Math.PI / 3;
            var phase3 = 4 * Math.PI / 3;

            for (var i = 0; i < range; ++i)
            {
                var r = Math.Sin(frequency * i + phase1) * amplitude + center;
                var g = Math.Sin(frequency * i + phase2) * amplitude + center;
                var b = Math.Sin(frequency * i + phase3) * amplitude + center;
                yield return new Tuple<int, int, int>((int)r, (int)g, (int)b);
            }
        }
    }
}
