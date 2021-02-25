using Colore.Data;
using Listener.Plugin.Razer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.Razer.DebugGrid
{
    public class DebugGrid : IRazerEffect
    {
        public string EffectName => "Debug Grid";

        public int RequiredSpectrumRange => 29;

        public void SetEffect(global::Colore.Effects.Virtual.IVirtualLedGrid virtualGrid, global::Colore.Data.Color firstColor, global::Colore.Data.Color secondaryColor, ICollection<global::Colore.Data.Color> albumColor, global::Colore.Data.Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            virtualGrid.Set(Color.Black);

            var keyboardGrid = virtualGrid.Where(x => x.Type == Colore.Effects.Virtual.KeyType.Keyboard);
            foreach (var key in keyboardGrid)
            {
                key.Color = Color.Red;
            }

            var mouseGrid = virtualGrid.Where(x => x.Type == Colore.Effects.Virtual.KeyType.Mouse);
            foreach (var key in mouseGrid)
            {
                key.Color = Color.Blue;
            }

            var mousepadGrid = virtualGrid.Where(x => x.Type == Colore.Effects.Virtual.KeyType.Mousepad);
            foreach (var key in mousepadGrid)
            {
                key.Color = Color.Purple;
            }

            var chromaLinkGrid = virtualGrid.Where(x => x.Type == Colore.Effects.Virtual.KeyType.ChromaLink);
            foreach (var key in chromaLinkGrid)
            {
                key.Color = Color.Green;
            }

            var headsetGrid = virtualGrid.Where(x => x.Type == Colore.Effects.Virtual.KeyType.Headset);
            foreach (var key in headsetGrid)
            {
                key.Color = Color.White;
            }
        }
    }
}
